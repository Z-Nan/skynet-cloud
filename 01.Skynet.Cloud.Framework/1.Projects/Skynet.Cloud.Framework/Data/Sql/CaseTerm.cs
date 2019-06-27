/*----------------------------------------------------------------
// Copyright (C) 2010 �����������Ƽ����޹�˾
// ��Ȩ���С� 
//
// �ļ�����
// �ļ�����������
//
// 
// ������ʶ��
//
// �޸ı�ʶ��
// �޸�������
//
// �޸ı�ʶ��
// �޸�������
//----------------------------------------------------------------*/
using System;

namespace UWay.Skynet.Cloud.Data
{
	/// <summary>
	/// Encapsulates a single WHEN ... THEN ... statement
    /// </summary>
    [Serializable]
	public class CaseTerm
	{
		WhereClause cond;
		SqlExpression val;

		/// <summary>
		/// Creates a new CaseTerm
		/// </summary>
		/// <param name="condition">Condition for the WHEN clause</param>
		/// <param name="val">Value for the THEN clause</param>
		public CaseTerm(WhereClause condition, SqlExpression val)
		{
			this.cond = condition;
			this.val = val;
		}

		internal WhereClause Condition
		{
			get { return this.cond; }
		}

		internal SqlExpression Value
		{
			get { return this.val; }
		}
	}
}
