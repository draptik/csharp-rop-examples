using System;
using System.Linq;
using FluentAssertions;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Xunit;

namespace RopDemo
{
    public class LaYumbaTests
    {

        [Fact]
        public void LaYumba_Option()
        {
            Func<int, Option<int>> f1 = x => x > 0 ? Some(x) : None;
            Func<int, Option<int>> f2 = x => x < 10 ? Some(x) : None;
            Func<int, Option<int>> f3 = x => x % 2 == 0 ? Some(x) : None;
            
            var optionResult1 = f1(2).Bind(f2).Bind(f3);
            optionResult1.Match(
                () => true.Should().BeFalse(),
                x => x.Should().Be(2));

            var optionResult2 = f1(11).Bind(f2).Bind(f3);
            optionResult2.Match(
                () => true.Should().BeTrue(),
                x => true.Should().BeFalse());
        }
        
        [Fact]
        public void LaYumba_Either()
        {
            // not really sure why we need the explicit cast..
            Func<int, Either<MyError, int>> f1 = x => 
                x > 0 
                    ? Right(x) 
                    : (Either<MyError, int>) Left(new MyError("must be larger than 0"));
            
            Func<int, Either<MyError, int>> f2 = x => 
                x < 10 
                    ? Right(x) 
                    : (Either<MyError, int>) Left(new MyError("must be less than 10"));
            
            Func<int, Either<MyError, int>> f3 = x => 
                x % 2 == 0 
                    ? Right(x) 
                    : (Either<MyError, int>) Left(new MyError("must be even"));

            var either1 = f1(2).Bind(f2).Bind(f3);
            either1.Match(
                e => true.Should().BeFalse(),
                x => x.Should().Be(2));

            var either2 = f1(11).Bind(f2).Bind(f3);
            either2.Match(
                e => e.Msg.Should().Be("must be less than 10"),
                x => true.Should().BeFalse());
        }

        [Fact]
        public void LaYumba_Validation()
        {
            Func<int, Validation<int>> f1 = x => 
                x > 0 
                    ? Valid(x) 
                    : Invalid(new MyError("must be larger than 0"));
            
            Func<int, Validation<int>> f2 = x => 
                x < 10 
                    ? Valid(x) 
                    : Invalid(new MyError("must be less than 10"));
            
            Func<int, Validation<int>> f3 = x => 
                x % 2 == 0 
                    ? Valid(x) 
                    : Invalid(new MyError("must be even"));

            var validation1 = f1(2).Bind(f2).Bind(f3);
            validation1.Match(
                e => true.Should().BeFalse(),
                x => x.Should().Be(2));

            var validation2 = f1(11).Bind(f2).Bind(f3);
            validation2.Match(
                e => ((MyError)e.First()).Msg.Should().Be("must be less than 10"),
                x => true.Should().BeFalse());
        }
        
        public class MyError : Error
        {
            public string Msg { get; }

            public MyError(string msg)
            {
                Msg = msg;
            }
        }
    }
}