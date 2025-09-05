using System;

namespace Domain.Common
{
    /// <summary>
    /// Thrown when an aggregate violates a domain rule given its current state.
    /// Use for invalid state transitions or invariant breaches (not bad arguments).
    /// </summary>
    public sealed class DomainRuleViolationException : Exception
    {
        public DomainRuleViolationException(string message) : base(message) { }
    }
}


