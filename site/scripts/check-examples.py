#!/usr/bin/env python3
"""Checks that the code shown on the site is code that compiled.

Every F# block in site/content must be an excerpt of a .fs file under site/examples.
With --js, every JavaScript block must be an excerpt of Fable output under artifacts/site-examples.

Usage: python3 site/scripts/check-examples.py [--js] [page.md ...]
"""
import html
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parents[2]
CONTENT = ROOT / "site" / "content"
FSHARP_SOURCES = ROOT / "site" / "examples"
JS_SOURCES = ROOT / "artifacts" / "site-examples"

FENCE = re.compile(r"^```(\w+)[^\n]*\n(.*?)^```[ \t]*$", re.M | re.S)
PRE = re.compile(r"<pre([^>]*)><code[^>]*>(.*?)</code></pre>", re.S)
TAG = re.compile(r"<[^>]+>")


def lines_of(text):
    return [line.rstrip() for line in text.replace("\r\n", "\n").split("\n")]


def trimmed(block):
    lines = lines_of(block)
    while lines and not lines[0].strip():
        lines.pop(0)
    while lines and not lines[-1].strip():
        lines.pop()
    return lines


def indent(line):
    return len(line) - len(line.lstrip(" "))


def is_excerpt(block, source):
    wanted = trimmed(block)
    if not wanted:
        return True
    have = lines_of(source)
    stripped = [line.strip() for line in wanted]
    for start in range(len(have) - len(wanted) + 1):
        if have[start].strip() != stripped[0]:
            continue
        window = have[start:start + len(wanted)]
        if [line.strip() for line in window] != stripped:
            continue
        offsets = {indent(w) - indent(x) for w, x in zip(window, wanted) if x.strip()}
        if len(offsets) <= 1:
            return True
    return False


def blocks(page_text):
    for match in FENCE.finditer(page_text):
        yield match.group(1).lower(), match.group(2)
    for match in PRE.finditer(page_text):
        language = "javascript" if 'data-lang="javascript"' in match.group(1) else "fsharp"
        yield language, html.unescape(TAG.sub("", match.group(2)))


def main(argv):
    check_js = "--js" in argv
    pages = [pathlib.Path(a).resolve() for a in argv if not a.startswith("--")]
    if not pages:
        pages = sorted(CONTENT.rglob("*.md"))
    fsharp = [(p, p.read_text()) for p in sorted(FSHARP_SOURCES.rglob("*.fs")) if "/obj/" not in str(p) and "/bin/" not in str(p)]
    js = [(p, p.read_text()) for p in sorted(JS_SOURCES.rglob("*.js")) if "fable_modules" not in str(p)] if check_js else []
    failures = 0
    counted = {"fsharp": 0, "javascript": 0}
    for page in pages:
        for language, block in blocks(page.read_text()):
            if language == "fsharp":
                sources = fsharp
            elif language == "javascript" and check_js:
                sources = js
            else:
                continue
            counted[language] += 1
            if not any(is_excerpt(block, text) for _, text in sources):
                failures += 1
                first = next((line for line in trimmed(block) if line.strip()), "")
                print(f"NOT COMPILED: {page.relative_to(ROOT)}: {language} block starting '{first.strip()[:70]}'")
    print(f"checked {counted['fsharp']} F# and {counted['javascript']} JavaScript blocks on {len(pages)} pages, {failures} unmatched")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
