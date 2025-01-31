using JetBrains.Annotations;

public abstract class StateTag<T, S>
    where T : Stateful<T, S>
    where S : StateTag<T, S> {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns>null if there is no state transition to be performed</returns>
    [CanBeNull] public abstract S Update(T self, float deltaTime);
}
