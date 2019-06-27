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

namespace UWay.Skynet.Cloud.Data.Reporting
{
	/// <summary>
	/// ʱ��ת��������
	/// </summary>
	public class FiscalQuarter
	{
		DateTime[] quarters = new DateTime[4];
		int year;

		static int startDay = 1, startMonth = 1;

		/// <summary>
		/// Creates a new FiscalQuarter
		/// </summary>
		public FiscalQuarter()
		{
			DateTime today = DateTime.Today;
			int fiscalQuarterMonth = startMonth;
			int fiscalQuarterDay = startDay;
			year = today.Year;
			quarters[0] = new DateTime(year, fiscalQuarterMonth, fiscalQuarterDay);
			quarters[1] = quarters[0].AddMonths(3);
			quarters[2] = quarters[0].AddMonths(6);
			quarters[3] = quarters[0].AddMonths(9);
		}

		/// <summary>
		/// ��ȡ��ǰ����
		/// </summary>
        /// <returns>�ڼ�����</returns>
		public int GetCurrentQuarter()
		{
			return GetQuarterForDate(DateTime.Today);
		}
		
		/// <summary>
		/// ��ȡ�����ڵļ���
		/// </summary>
		/// <param name="date">ʱ�䣬���磺2010-10-11</param>
        /// <returns>�ڼ�����</returns>
		public int GetQuarterForDate(DateTime date)
		{
			DateTime thisYearDate = new DateTime(year, date.Month, date.Day);
			for (int i = 0; i < 3; i++)
			{
				if (quarters[i] <= thisYearDate && thisYearDate < quarters[i+1])
					return i + 1;
			}
			return 4;
		}

		/// <summary>
		/// ��ȡ���ȵĿ�ʼʱ�� ���磺2010���3���ȷ���2010-09-01
		/// </summary>
		/// <param name="quarter">����</param>
		/// <param name="year">��</param>
        /// <returns>���ȵĿ�ʼʱ��</returns>
		public DateTime GetStartDate(int quarter, int year)
		{
			DateTime date = quarters[quarter - 1];
			return new DateTime(year, date.Month, date.Day);
		}

		/// <summary>
        /// ��Ŀ�ʼ�������������в��ֹ�˾�Ŀ�������10�ſ�ʼ�ģ���StartDay����Ϊ10����ô1�µ�ͳ�����ڽ���1��10�յ�2��10��
		/// </summary>
		public static int StartDay
		{
			get { return startDay; }
			set { startDay = value; }
		}

		/// <summary>
        ///�µĿ�ʼ�·ݣ��������в��ֹ�˾���걨����2�¿�ʼ�ģ���StartMonth����Ϊ2����ô2010���ͳ�����ڽ���2010��2�µ�2011��2��
		/// </summary>
		public static int StartMonth
		{
			get { return startMonth; }
			set { startMonth = value; }
		}
	}
}
