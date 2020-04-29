using System;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace RopDemo
{
    public class CSharpFunctionalTests
    {
        [Fact]
        public void CSharpFunctional_Maybe()
        {
            Func<int, Maybe<int>> f1 = x => 
                x > 0 
                    ? Maybe<int>.From(x) 
                    : Maybe<int>.None;
            
            Func<int, Maybe<int>> f2 = x => 
                x < 10 
                    ? Maybe<int>.From(x) 
                    : Maybe<int>.None;
            
            Func<int, Maybe<int>> f3 = x => 
                x % 2 == 0 
                    ? Maybe<int>.From(x) 
                    : Maybe<int>.None;

            var maybe = f1(2).Bind(f2).Bind(f3);
            maybe.Value.Should().Be(2);

            var anotherMaybe = f1(11).Bind(f2).Bind(f3);
            anotherMaybe.HasNoValue.Should().BeTrue();
        }

        [Fact]
        public void CSharpFunctional_Result()
        {
            Func<int, Result<int, MyError>> f1 = x => 
                x > 0 
                    ? Result.Success<int, MyError>(x) 
                    : Result.Failure<int, MyError>(new MyError("must be larger than 0"));
            
            Func<int, Result<int, MyError>> f2 = x => 
                x < 10 
                    ? Result.Success<int, MyError>(x) 
                    : Result.Failure<int, MyError>(new MyError("must be less than 10"));
            
            Func<int, Result<int, MyError>> f3 = x => 
                x % 2 == 0 
                    ? Result.Success<int, MyError>(x) 
                    : Result.Failure<int, MyError>(new MyError("must be even"));

            var result1 = f1(2).Bind(f2).Bind(f3);
            result1.Should().BeSuccess();
            result1.Value.Should().Be(2);

            var result2 = f1(11).Bind(f2).Bind(f3);
            result2.Should().BeFailure();
            result2.Error.Msg.Should().Be("must be less than 10");
        }
    }
    
    public class MyError
    {
        public string Msg { get; }

        public MyError(string msg)
        {
            Msg = msg;
        }
    }
}
