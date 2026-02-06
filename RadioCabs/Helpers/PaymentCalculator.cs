namespace RadioCabs.Helpers
{
    public static class PaymentCalculator
    {
        public static int GetCompanyAmount(string paymentType)
        {
            return paymentType == "Quarterly" ? 40 : 15;
        }

        public static int GetDriverAmount(string paymentType)
        {
            return paymentType == "Quarterly" ? 25 : 10;
        }

        public static int GetAdvertisementAmount(string paymentType)
        {
            return paymentType == "Quarterly" ? 40 : 15;
        }
    }
}
