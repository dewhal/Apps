using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SidNWhalSoft.Apps.BudgetPal
{
    public enum PaymentType
    {
        Income,
        Expense
    };
    
    public enum  PaymentCategory
    { 
        Fixed,
        Mandatory,
        Discretionary,
        Variable,
        Optional,
        Target
    };

    public enum PaymentFrequency
    {
        Weekly = 0,
        BiWeekly =1,
        Monthly = 2,
        BiMonthly = 3,
        Quarterly = 4,
        Annually = 5
    };

    public class Payment
    {
        private int _dateOfMonthPaymentMade;
        private string _nameOfPayment;
        private double _amount;
        private PaymentFrequency _paymentFrequency;
        private DateTime _nextPaymentDate;
        private PaymentCategory _paymentCategory;
        protected PaymentType _paymentType;

        private object[,] paymentFrequencyMap = new object[,] { {"W", 0}, {"B", 1}, {"M", 2}, {"BI", 3}, {"Q", 4}, {"A",5} } ;

        public Payment(string name, int dateOfMonth, double paymentAmount)
        {
            DateTime paymentDate;
            int daysFromWeekendDate;

            this._nameOfPayment = name;
            this._amount = paymentAmount;
            this._dateOfMonthPaymentMade = dateOfMonth;

            if (DateTime.Now.Day >= _dateOfMonthPaymentMade)
                paymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, _dateOfMonthPaymentMade);
            else
                paymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _dateOfMonthPaymentMade);

            if (paymentDate.DayOfWeek == DayOfWeek.Saturday)
                if (this._paymentType == PaymentType.Income)
                    paymentDate.AddDays(-1);
                else
                    paymentDate.AddDays(2);
            else if ( paymentDate.DayOfWeek == DayOfWeek.Sunday)
                if (this._paymentType == PaymentType.Income)
                    paymentDate.AddDays(-2);
                else
                    paymentDate.AddDays(1);

            this._nextPaymentDate = paymentDate;
        }

        public Payment(string name, int dateOfMonth, double paymentAmount, PaymentCategory category)
            :this(name,dateOfMonth,paymentAmount)
        {
            this._paymentCategory = category;
        }

        public Payment(string name, int dateOfMonth, double paymentAmount,string frequency, PaymentCategory category)
            : this(name, dateOfMonth, paymentAmount,category)
        {
            this._paymentFrequency = (PaymentFrequency) Enum.Parse(typeof(PaymentFrequency),frequency.ToString(),true);
        }

        public string Name
        {
            get { return this._nameOfPayment; }
            set { this._nameOfPayment = value; }
        }

        public double Amount
        {
            get { return this._amount; }
            set { this._amount = value; }
        }

        public PaymentFrequency Frequency
        {
            get { return this._paymentFrequency; }
            set { this._paymentFrequency = (PaymentFrequency) Enum.Parse(typeof(PaymentFrequency), value.ToString(), true); }
        }

        public virtual DateTime NextPaymentDate
        {
            get { return this._nextPaymentDate; }
        }

        public bool IsQuarterly
        {
            get {
                return PaymentFrequency.Quarterly == this._paymentFrequency;
            }
        }

        public virtual double NextPaymentAmount
        {
            get
            {
                if (this._nextPaymentDate <= DateTime.Now)
                    return 0;
                else
                {
                    if (this._nextPaymentDate.Month == DateTime.Now.Month)
                        return _amount;
                    else
                        return 0;
                }
            }
        }

        public PaymentCategory CategoryOfPayment
        {
            get { return this._paymentCategory; }
            set { this._paymentCategory = value; }
        }

        public PaymentType TypeOfPayment
        {
            get { return this._paymentType; }
            set { this._paymentType = value; }
        }

        internal int MapPaymentFrequency(string paymentFrequencyCode)
        {
            for (int i = 0; i < paymentFrequencyMap.GetLength(0); i++)
            {
                if ((string)paymentFrequencyMap[i,0] == (string)paymentFrequencyCode)
                    return (int)paymentFrequencyMap[i,1];
            }

            return -1;
        }
    }

    public class Expense:Payment
    {
        public Expense(string name, int dateOfMonth, double paymentAmount, PaymentCategory category)
            :base(name, dateOfMonth, paymentAmount, category)
        {
            this.TypeOfPayment = PaymentType.Expense;
        }

        public Expense(string name, int dateOfMonth, double paymentAmount,string frequency, PaymentCategory category)
            : this(name, dateOfMonth, paymentAmount, category)
        {
            this.Frequency = (PaymentFrequency)MapPaymentFrequency(frequency);
        }

        //public override DateTime NextPaymentDate
        //{
        //    get { return this.NextPaymentDate; }
        //}
    }

    public class Income : Payment
    {
        public Income(string name, int dateOfMonthPaymentMade, double incomeAmount, PaymentCategory category)
            : base(name, dateOfMonthPaymentMade, incomeAmount, category)
        {
            this.TypeOfPayment = PaymentType.Income;
        }

        public Income(string name, int dateOfMonth, double paymentAmount,string frequency, PaymentCategory category)
            : this(name, dateOfMonth, paymentAmount, category)
        {
            //this.Frequency = (PaymentFrequency)Enum.Parse(typeof(PaymentFrequency), frequency.ToString(),true);
            this.Frequency = (PaymentFrequency) MapPaymentFrequency(frequency);
        }

        public override DateTime NextPaymentDate
        {
            get
            {
                return base.NextPaymentDate;
            }
        }
    }

}
