﻿using System.Collections.Generic;
using IbanNet.Registry;
using IbanNet.Validation.Methods;
using IbanNet.Validation.Rules;

namespace IbanNet
{
	/// <summary>
	/// Options for <see cref="IbanValidator"/>.
	/// </summary>
	public class IbanValidatorOptions
	{
		/// <summary>
		/// Gets or sets the IBAN country registry factory. Defaults to <see cref="IbanRegistry.Default"/>.
		/// </summary>
		public IIbanRegistry Registry { get; set; } = IbanRegistry.Default;

		/// <summary>
		/// Gets or sets the validation method. Defaults to <see cref="StrictValidation"/>.
		/// </summary>
		public ValidationMethod ValidationMethod { get; set; } = new StrictValidation();

		/// <summary>
		/// Gets or sets custom rules to apply after built-in IBAN validation has taken place.
		/// </summary>
		public ICollection<IIbanValidationRule> Rules { get; set; } = new List<IIbanValidationRule>();
	}
}
