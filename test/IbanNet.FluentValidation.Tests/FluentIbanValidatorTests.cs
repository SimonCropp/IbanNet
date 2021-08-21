﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using IbanNet.Validation.Results;
using Moq;
using TestHelpers;
using Xunit;

namespace IbanNet.FluentValidation
{
    [Collection(nameof(SetsStaticValidator))]
    public class FluentIbanValidatorTests
    {
        private readonly IbanValidatorStub _ibanValidatorStub;
        private readonly TestModelValidator _validator;

        protected FluentIbanValidatorTests()
        {
            _ibanValidatorStub = new IbanValidatorStub();
            var sut = new FluentIbanValidator<TestModel>(_ibanValidatorStub);
            _validator = new TestModelValidator(sut);
        }

        public class When_validating_an_invalid_iban : FluentIbanValidatorTests
        {
            private const string AttemptedIbanValue = TestValues.InvalidIban;

            [Fact]
            public void It_should_call_validator()
            {
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                // Act
                _validator.Validate(obj);

                // Assert
                _ibanValidatorStub.Verify(m => m.Validate(AttemptedIbanValue), Times.Once);
            }

            [Fact]
            public void It_should_fail()
            {
                const string expectedPropertyName = "Bank Account Number";
                string expectedErrorMessage = $"'{expectedPropertyName}' is not a valid IBAN.";
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                // Act
                IEnumerable<ValidationFailure> actual = _validator.ShouldHaveValidationErrorFor(x => x.BankAccountNumber, obj);

                // Assert
                ValidationFailure error = actual.Should()
                    .HaveCount(1, "because one validation error should have occurred")
                    .And.Subject.First();
                error.FormattedMessagePlaceholderValues.Should()
                    .ContainKey("Error")
                    .WhichValue.Should()
                    .BeOfType<IllegalCharactersResult>();
                error.ErrorMessage.Should().Be(expectedErrorMessage);
            }
        }

        public class When_validating_a_valid_iban : FluentIbanValidatorTests
        {
            private const string AttemptedIbanValue = TestValues.ValidIban;


            [Fact]
            public void It_should_call_validator()
            {
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                // Act
                _validator.Validate(obj);

                // Assert
                _ibanValidatorStub.Verify(m => m.Validate(AttemptedIbanValue), Times.Once);
            }

            [Fact]
            public void It_should_succeed()
            {
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                _validator.ShouldNotHaveValidationErrorFor(x => x.BankAccountNumber, obj);
            }
        }

        public class When_validating_a_null_value : FluentIbanValidatorTests
        {
            private const string AttemptedIbanValue = null;


            [Fact]
            public void It_should_not_call_validator()
            {
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                // Act
                _validator.Validate(obj);

                // Assert
                _ibanValidatorStub.Verify(m => m.Validate(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public void It_should_succeed()
            {
                var obj = new TestModel { BankAccountNumber = AttemptedIbanValue };

                _validator.ShouldNotHaveValidationErrorFor(x => x.BankAccountNumber, obj);
            }
        }

        public class When_validator_is_null
        {
            [Fact]
            public void It_should_throw()
            {
                IIbanValidator ibanValidator = null;

                // Act
                // ReSharper disable once AssignNullToNotNullAttribute
                Func<FluentIbanValidator<TestModel>> act = () => new FluentIbanValidator<TestModel>(ibanValidator);

                // Assert
                act.Should()
                    .Throw<ArgumentNullException>()
                    .Which.ParamName.Should()
                    .Be(nameof(ibanValidator));
            }
        }

        public class When_validator_context_is_null : FluentIbanValidatorTests
        {
            [Fact]
            public void It_should_not_throw_and_validate_successfully()
            {
                var fluentIbanValidator = new FluentIbanValidator<TestModel>(new IbanValidator());
                ValidationContext<TestModel> context = null;

                // Act
                // ReSharper disable once AssignNullToNotNullAttribute
                Func<bool> act = () => fluentIbanValidator.IsValid(context, string.Empty);

                // Assert
                act.Should().NotThrow();
            }
        }

        private class TestModelValidator : AbstractValidator<TestModel>
        {
            public TestModelValidator(FluentIbanValidator<TestModel> validator)
            {
                RuleFor(x => x.BankAccountNumber).SetValidator(validator);
            }
        }
    }
}
