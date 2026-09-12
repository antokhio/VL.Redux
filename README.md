# VL.Redux (WIP)

An experiment with an MVU approach for vvvv.

![overview](assets/01-overview.png)

The idea is to define state as slice records:

![state slice](assets/02-state-slice.png)

Slices can define reusable state mutations or mutations directly in place.
The store combines multiple slices:

![state slice](assets/03-store-setup.png)

All slices must be known when the store is created.
Actions can then be dispatched by state type:

![dispatch](assets/04-dispatch.png)

Subscribe to receive state updates:

![subscribe](assets/05-subscribe.png)

The model holds the state, while the view creates channels and dispatches actions. Each action passes through the MVU pipeline and produces an updated state.

This is still experimental, but it works well in the vvvv context.

Current limitations:

- Each slice must use a unique type.
- All slices must be known at application startup.
- This is a minimal proof of concept without middleware, logging, or development tools.
- State is not serializable by default.


Notice: 

This is not an `agnostic` redux implementation, but more of `Reactive` state management tool.