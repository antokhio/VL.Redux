# Copilot Instructions

## Project Guidelines

- vvvv gamma patches `.vl` files are hand-authored in the vvvv IDE by user. Don't read or edit them, they are outside of scope.
- Never create or use throwaway probe/console/sample projects to test behavior unless I explicitly request it. Your default—and only autonomous—verification step is to build the affected project. If a build is insufficient, explain the unverified behavior and the exact probe you would propose, then stop and wait for approval.

## API Design Principles

- For VL.Redux, prioritize a minimal, clean functional API for VL patches to define slices and dispatch actions. Avoid unnecessary wrapper types and abstractions; discuss API design before implementation when requested. Support actions authored as static methods returning `IAction` or as `Func<TModel, TModel>`. Use `TModel` for model generic parameters; `TState` is reserved by the project's conventions. Discuss design without editing code until requested.

## Available MCPs

- vvvv/vl standard libraries: `VL.Core`, `VL.Lib`, `VL.Skia`, `VL.Stride.*`, `IChannel`/`Channel`, `Spread<T>`, `NodeContext`, `AppHost`, `IVLTypeInfo`.
