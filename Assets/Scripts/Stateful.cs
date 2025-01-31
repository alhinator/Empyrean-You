using System;
using UnityEngine;

public abstract class Stateful<T, S> : MonoBehaviour
    where T : Stateful<T, S>
    where S : StateTag<T, S> {

    private S _state;

    public abstract S InitialState();
    
    protected void Start() {
        this._state = InitialState();
    }

    protected void Update() {
        var result = this._state.Update((T) this, Time.deltaTime);
        if(result != null) this._state = result;
    }
}
