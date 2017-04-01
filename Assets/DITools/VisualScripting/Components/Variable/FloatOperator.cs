using UnityEngine;
using System.Collections;

namespace DI.VisualScripting
{
	[VisualComponent("Variable")]
	public class FloatOperator : DIVisualComponent {

		public OperatorType operatorType;
		public GetFloat variable1, variable2, storeVariable;

		protected override void WhatDo()
		{
			if (operatorType == OperatorType.Add)
				storeVariable.value = variable1.value + variable2.value;
			else if (operatorType == OperatorType.Substract)
				storeVariable.value = variable1.value - variable2.value;
			else if (operatorType == OperatorType.Multiply)
				storeVariable.value = variable1.value * variable2.value;
			else if (operatorType == OperatorType.Divide)
				storeVariable.value = variable1.value / variable2.value;
			else if (operatorType == OperatorType.Modulo)
				storeVariable.value = variable1.value % variable2.value;

			Finish();
		}

	}
}
