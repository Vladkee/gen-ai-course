namespace BankingApi.Validation;

public static class Iso4217Currencies
{
    private static readonly HashSet<string> ValidCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "AED","AFN","ALL","AMD","ANG","AOA","ARS","AUD","AWG","AZN",
        "BAM","BBD","BDT","BGN","BHD","BIF","BMD","BND","BOB","BOV",
        "BRL","BSD","BTN","BWP","BYN","BZD","CAD","CDF","CHE","CHF",
        "CHW","CLF","CLP","CNY","COP","COU","CRC","CUC","CUP","CVE",
        "CZK","DJF","DKK","DOP","DZD","EGP","ERN","ETB","EUR","FJD",
        "FKP","GBP","GEL","GHS","GIP","GMD","GNF","GTQ","GYD","HKD",
        "HNL","HRK","HTG","HUF","IDR","ILS","INR","IQD","IRR","ISK",
        "JMD","JOD","JPY","KES","KGS","KHR","KMF","KPW","KRW","KWD",
        "KYD","KZT","LAK","LBP","LKR","LRD","LSL","LYD","MAD","MDL",
        "MGA","MKD","MMK","MNT","MOP","MRU","MUR","MVR","MWK","MXN",
        "MXV","MYR","MZN","NAD","NGN","NIO","NOK","NPR","NZD","OMR",
        "PAB","PEN","PGK","PHP","PKR","PLN","PYG","QAR","RON","RSD",
        "RUB","RWF","SAR","SBD","SCR","SDG","SEK","SGD","SHP","SLL",
        "SOS","SRD","STN","SVC","SYP","SZL","THB","TJS","TMT","TND",
        "TOP","TRY","TTD","TWD","TZS","UAH","UGX","USD","USN","UYI",
        "UYU","UYW","UZS","VES","VND","VUV","WST","XAF","XCD","XOF",
        "XPF","YER","ZAR","ZMW","ZWL"
    };

    public static bool IsValid(string code) =>
        !string.IsNullOrWhiteSpace(code) && ValidCodes.Contains(code);
}
