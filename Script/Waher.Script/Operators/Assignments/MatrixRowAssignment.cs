﻿using System;
using System.Collections.Generic;
using System.Text;
using Waher.Script.Model;
using Waher.Script.Operators.Matrices;

namespace Waher.Script.Operators.Assignments
{
	/// <summary>
	/// Matrix Row Assignment operator.
	/// </summary>
	public class MatrixRowAssignment : UnaryOperator 
	{
		RowVector matrixRow;

		/// <summary>
		/// Matrix Row Assignment operator.
		/// </summary>
		/// <param name="MatrixRow">Matrix Row</param>
		/// <param name="Operand">Operand.</param>
		/// <param name="Start">Start position in script expression.</param>
		/// <param name="Length">Length of expression covered by node.</param>
		public MatrixRowAssignment(RowVector MatrixRow, ScriptNode Operand, int Start, int Length)
			: base(Operand, Start, Length)
		{
			this.matrixRow = MatrixRow;
		}

		/// <summary>
		/// Matrix Row.
		/// </summary>
		public RowVector MatrixRow
		{
			get { return this.matrixRow; }
		}

	}
}