using System;
using FluentAssertions;
using Xunit;
using LanguageExt;
using LanguageExt.UnitTesting;
using static LanguageExt.Prelude;

namespace RopDemo
{
    public class LanguageExtTests
    {
        [Fact]
        public void LanguageExt_Option()
        {
            Func<int, Option<int>> f1 = x => x > 0 ? Some(x) : None;
            Func<int, Option<int>> f2 = x => x < 10 ? Some(x) : None;
            Func<int, Option<int>> f3 = x => x % 2 == 0 ? Some(x) : None;

            var optResult = f1(2).Bind(f2).Bind(f3);
            optResult.ShouldBeSome(x => x.Should().Be(2));

            var optResult2 = f1(11).Bind(f2).Bind(f3);
            optResult2.ShouldBeNone();

            // alternative syntax
            var optResult3 =
                from x in f1(11)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            optResult3.ShouldBeNone();

            var optResult4 =
                from x in f1(2)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            optResult4.ShouldBeSome(x => x.Should().Be(2));
        }

        [Fact]
        public void LanguageExt_Either()
        {
            Func<int, Either<Error, int>> f1 = x => 
                x > 0 
                    ? Right<Error, int>(x) 
                    : Left<Error, int>(Error.New("must be larger than 0"));
            
            Func<int, Either<Error, int>> f2 = x => 
                x < 10 
                    ? Right<Error, int>(x) 
                    : Left<Error, int>(Error.New("must be less than 10"));
            
            Func<int, Either<Error, int>> f3 = x => 
                x % 2 == 0 
                    ? Right<Error, int>(x) 
                    : Left<Error, int>(Error.New("must be even"));

            var eitherResult1 = f1(2).Bind(f2).Bind(f3);
            eitherResult1.ShouldBeRight(x => x.Should().Be(2));
            
            var eitherResult2 = f1(11).Bind(f2).Bind(f3);
            eitherResult2.ShouldBeLeft(x => x.Value.Should().Be("must be less than 10"));

            // alternative syntax
            var eitherResult3 =
                from x in f1(2)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            eitherResult3.ShouldBeRight(x => x.Should().Be(2));

            var eitherResult4 =
                from x in f1(11)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            eitherResult4.ShouldBeLeft(x => x.Value.Should().Be("must be less than 10"));
        }

        [Fact]
        public void LanguageExt_Validation()
        {
            Func<int, Validation<Error, int>> f1 = x => 
                x > 0 
                    ? Success<Error, int>(x) 
                    : Fail<Error, int>(Error.New("must be larger than 0"));
            
            Func<int, Validation<Error, int>> f2 = x => 
                x < 10 
                    ? Success<Error, int>(x) 
                    : Fail<Error, int>(Error.New("must be less than 10"));
            
            Func<int, Validation<Error, int>> f3 = x => 
                x % 2 == 0 
                    ? Success<Error, int>(x) 
                    : Fail<Error, int>(Error.New("must be even"));

            var validation1 = f1(2).Bind(f2).Bind(f3);
            validation1.ShouldBeSuccess(x => x.Should().Be(2));
            
            var validation2 = f1(11).Bind(f2).Bind(f3);
            validation2.ShouldBeFail(x => x.First().Value.Should().Be("must be less than 10"));

            // alternative syntax
            var validation3 =
                from x in f1(2)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            validation3.ShouldBeSuccess(x => x.Should().Be(2));

            var validation4 =
                from x in f1(11)
                from y in f2(x)
                from z in f3(y)
                select z;
            
            validation4.ShouldBeFail(x => x.First().Value.Should().Be("must be less than 10"));
        }

        public class Error : NewType<Error, string>
        {
            public Error(string e) : base(e)
            {
            }
        }
    }
}
