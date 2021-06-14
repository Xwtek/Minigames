using System;
using System.Security.Permissions;
using System.Runtime.Serialization;
[System.Serializable]
public class DiceException : System.Exception
{
    public DiceFailure Failure { get; }
    public DiceException() { }
    public DiceException(DiceFailure failure) : this(MessageOnFailure(failure), failure) { }
    public DiceException(string message) : base(message) { }
    public DiceException(string message, DiceFailure failure) : base(message) { Failure = failure; }
    public DiceException(string message, System.Exception inner) : base(message, inner) { }
    public DiceException(string message, DiceFailure failure, System.Exception inner) : base(message, inner) {Failure = failure; }
    protected DiceException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) {
        Failure = (DiceFailure)info.GetInt32("Failure");
    }
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException("info");
        }

        info.AddValue("Failure", this.Failure);

        // MUST call through to the base class to let it save its own state
        base.GetObjectData(info, context);
    }
    private static string MessageOnFailure(DiceFailure failure)
    {
        switch (failure)
        {
            case DiceFailure.NoRecord:
                return "No Dice recording yet. You may never rolled or the last roll is an error";
            case DiceFailure.IsRecording:
                return "The dice is still recording the possible result";
            case DiceFailure.IsReplaying:
                return "The dice is still replaying the result";
            default:
                return "Unknown error occured";
        }
    }
}

public enum DiceFailure
{
    Other = 0,
    IsRecording = 1,
    NoRecord = 2,
    IsReplaying=3,
}
