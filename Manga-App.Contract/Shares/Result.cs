
using MangaApp.Contract.Abstractions;
using MangaApp.Contract.Shares.Errors;

namespace MangaApp.Contract.Shares;


public readonly partial struct Result<TValue> : IResult<TValue>, IResult
{
    private readonly TValue? _value;
    private readonly List<Error>? _errors;

    private Result(TValue value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
        _errors = null;
        IsError = false;
    }

    private Result(Error error)
    {
        _errors = new List<Error> { error };
        _value = default;
        IsError = true;
    }

    private Result(List<Error> errors)
    {
        _errors = errors.Count > 0 ? errors : throw new ArgumentException("Cannot create a Result<TValue> from an empty collection of errors. Provide at least one error.", nameof(errors));
        _value = default;
        IsError = true;
    }

    public bool IsError { get; }

    public List<Error> Errors
    {
        get
        {
            if (!IsError)
            {
                return new List<Error> { Error.Unexpected("Result.NoErrors", "Error list cannot be retrieved from a successful Result.") };
            }
            return _errors!;
        }
    }

    public List<Error> ErrorsOrEmptyList => IsError ? _errors! : new List<Error>();

    public TValue Value
    {
        get
        {
            if (IsError)
            {
                throw new InvalidOperationException("The Value property cannot be accessed when errors have been recorded. Check IsError before accessing Value.");
            }
            return _value!;
        }
    }
    public Error FirstError
    {
        get
        {
            if (!IsError)
            {
                throw new InvalidOperationException("The FirstError property cannot be accessed when no errors have been recorded. Check IsError before accessing FirstError.");
            }
            return _errors![0];
        }
    }

    List<Error>? IResult.Errors => throw new NotImplementedException();

    public static Result<TValue> From(List<Error> errors) => new Result<TValue>(errors);

    public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);

    public static implicit operator Result<TValue>(List<Error> errors) => new Result<TValue>(errors);

    public static implicit operator Result<TValue>(Error error) => new Result<TValue>(error);

    public void Switch(Action<TValue> onValue, Action<List<Error>> onError)
    {
        if (IsError)
        {
            onError(Errors);
        }
        else
        {
            onValue(Value);
        }
    }

    public async Task SwitchAsync(Func<TValue, Task> onValue, Func<List<Error>, Task> onError)
    {
        if (IsError)
        {
            await onError(Errors).ConfigureAwait(false);
        }
        else
        {
            await onValue(Value).ConfigureAwait(false);
        }
    }



    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<List<Error>, TResult> onError)
    {
        return IsError ? onError(Errors) : onValue(Value);
    }

    public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<List<Error>, Task<TResult>> onError)
    {
        return IsError ? await onError(Errors).ConfigureAwait(false) : await onValue(Value).ConfigureAwait(false);
    }

    public TResult MatchFirst<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onFirstError)
    {
        return IsError ? onFirstError(FirstError) : onValue(Value);
    }

    public async Task<TResult> MatchFirstAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<Error, Task<TResult>> onFirstError)
    {
        return IsError ? await onFirstError(FirstError).ConfigureAwait(false) : await onValue(Value).ConfigureAwait(false);
    }

    public Result<TResult> Then<TResult>(Func<TValue, Result<TResult>> onValue)
    {
        return IsError ? Errors : onValue(Value);
    }

    public async Task<Result<TResult>> ThenAsync<TResult>(Func<TValue, Task<Result<TResult>>> onValue)
    {
        return IsError ? Errors : await onValue(Value).ConfigureAwait(false);
    }

    public TValue Else(Func<List<Error>, TValue> onError)
    {
        return IsError ? onError(Errors) : Value;
    }

    public TValue Else(TValue onError)
    {
        return IsError ? onError : Value;
    }

    public async Task<TValue> ElseAsync(Func<List<Error>, Task<TValue>> onError)
    {
        return IsError ? await onError(Errors).ConfigureAwait(false) : Value;
    }

    public async Task<TValue> ElseAsync(Task<TValue> onError)
    {
        return IsError ? await onError.ConfigureAwait(false) : Value;
    }

    private bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append("IsError = ").Append(IsError).Append(", ");
        builder.Append("Errors = ").Append(string.Join(", ", Errors)).Append(", ");
        builder.Append("ErrorsOrEmptyList = ").Append(string.Join(", ", ErrorsOrEmptyList)).Append(", ");
        builder.Append("Value = ").Append(Value).Append(", ");
        builder.Append("FirstError = ").Append(FirstError);
        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = EqualityComparer<TValue>.Default.GetHashCode(_value);
            hashCode = (hashCode * 397) ^ (IsError ? EqualityComparer<List<Error>>.Default.GetHashCode(_errors) : 0);
            return hashCode;
        }
    }

    public bool Equals(Result<TValue> other)
    {
        return EqualityComparer<TValue>.Default.Equals(_value, other._value) &&
               EqualityComparer<List<Error>>.Default.Equals(_errors, other._errors) &&
               IsError == other.IsError;
    }

    public override bool Equals(object? obj) => obj is Result<TValue> other && Equals(other);
}