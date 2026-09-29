#!/usr/bin/env python3
"""Exercise packaged, grouped clients against a local HTTP server, without credentials."""
import argparse
import base64
from email.parser import BytesParser
from email.policy import default
import http.server
import json
from pathlib import Path
import subprocess
import threading

ROOT = Path(__file__).resolve().parents[3]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--bindings-root", type=Path, default=ROOT)
    parser.add_argument("--run-directory", type=Path, required=True)
    args = parser.parse_args()
    run = args.run_directory.resolve()
    run.mkdir(parents=True, exist_ok=False)
    requests = []

    class Handler(http.server.BaseHTTPRequestHandler):
        def reply(self, status, body):
            data = json.dumps(body).encode()
            self.send_response(status)
            self.send_header("Content-Type", "application/json")
            self.send_header("Content-Length", str(len(data)))
            self.end_headers()
            self.wfile.write(data)

        def do_GET(self):
            requests.append({"method": "GET", "path": self.path})
            self.reply(200, {"success": True, "errors": [], "messages": [],
                             "result": [{"id": "account-fixture", "name": "A&B", "type": "standard"}]})

        def do_POST(self):
            body = self.rfile.read(int(self.headers["Content-Length"]))
            (run / f"request-{len(requests)}.bin").write_bytes(body)
            message = BytesParser(policy=default).parsebytes(b"Content-Type: " + self.headers["Content-Type"].encode() + b"\r\nMIME-Version: 1.0\r\n\r\n" + body)
            parts = [{"name": p.get_param("name", header="Content-Disposition"), "media": p.get_content_type(),
                      "filename": p.get_filename(), "bodyBase64": base64.b64encode(p.get_payload(decode=True)).decode()} for p in message.iter_parts()]
            requests.append({"method": "POST", "path": self.path, "parts": parts})
            index = len(requests)
            self.reply(201 if index == 2 else 429 if index == 3 else 599,
                       {"success": index == 2, "errors": [], "messages": [], "result": {"jwt": "completion-fixture"}})

        def log_message(self, *_):
            pass

    with http.server.ThreadingHTTPServer(("127.0.0.1", 0), Handler) as server:
        thread = threading.Thread(target=server.serve_forever, daemon=True)
        thread.start()
        try:
            command = ["dotnet", "run", "--project", str(ROOT / "tests/HawaiiApi/HawaiiApi.fsproj"), "--configuration", "Release",
                       f"-p:HawaiiBindingsRoot={args.bindings_root.resolve()}", "--", f"http://127.0.0.1:{server.server_port}"]
            with (run / "runtime.log").open("w") as log:
                result = subprocess.run(command, cwd=ROOT, stdout=log, stderr=subprocess.STDOUT)
            if result.returncode:
                raise RuntimeError(f"Grouped client runtime failed; see {run / 'runtime.log'}")
        finally:
            server.shutdown()
            thread.join()
            (run / "requests.json").write_text(json.dumps(requests, indent=2) + "\n")
    assert len(requests) == 4, requests
    assert requests[0] == {"method": "GET", "path": "/accounts?name=A%26B"}
    for request in requests[1:]:
        assert request["path"] == "/accounts/account-fixture/workers/assets/upload?base64=true"
        assert request["parts"] == [{"name": "asset-hash", "media": "application/wasm", "filename": None,
                                     "bodyBase64": base64.b64encode(base64.b64encode(bytes([0, 255, 128, 65]))).decode()}]
    print(f"Four loopback HTTP exchanges and shared model ownership verified. Logs: {run}")


if __name__ == "__main__":
    main()
