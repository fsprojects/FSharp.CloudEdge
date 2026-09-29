module ModelSwitch

open Fable.Core

module Workers = FSharp.CloudEdge.Runtime.Workers
module WorkersAI = FSharp.CloudEdge.Runtime.WorkersAIProvider
module Compatible = FSharp.CloudEdge.Support.AI.V4.OpenAICompatible
module V4 = FSharp.CloudEdge.Support.AI.V4.Provider

type Env =
    abstract AI: Workers.Ai<Workers.AiModels>
    abstract LLM_URL: string
    abstract LLM_KEY: string
    abstract LLM_MODEL: string

let workersModel (env: Env) : V4.LanguageModelV4 =
    let settings = WorkersAI.WorkersAISettings2.Create(binding = env.AI)
    let workersai = WorkersAI.Exports.createWorkersAI(U2.Case1 settings)
    workersai.chat(U2.Case1 "@cf/meta/llama-3.3-70b-instruct-fp8-fast")

let endpointModel (env: Env) : V4.LanguageModelV4 =
    let settings =
        Compatible.OpenAICompatibleProviderSettings.Create(
            baseURL = env.LLM_URL,
            name = "own-llm",
            apiKey = env.LLM_KEY
        )
    let endpoint = Compatible.Exports.createOpenAICompatible settings
    endpoint.chatModel env.LLM_MODEL

let modelFor (env: Env) (plan: string) =
    if plan = "pro" then endpointModel env else workersModel env
