namespace Profitocracy.Core.Domain.Model.Shared.ValueObjects;

public class Currency : IEquatable<Currency>
{
    private Currency(string code, string symbol)
    {
        Code = code;
        Symbol = symbol;
    }
    
    public string Code { get; }
    public string Symbol { get; }
    
    public bool Equals(Currency? cur)
    {
        return cur?.Code == Code;
    }

    public static bool operator ==(Currency cur1, Currency cur2)
    {
        return cur1.Code.Equals(cur2.Code);
    }

    public static bool operator !=(Currency cur1, Currency cur2)
    {
        return !cur1.Code.Equals(cur2.Code);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Currency other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Symbol);
    }
    
    /// We force other layers of the project to use
    /// AvailableCurrencies instead of creating own currencies
    public static class AvailableCurrencies
    {
        // -- Major Global Currencies --
        public static readonly Currency Usd = new("USD", "$"); // United States Dollar
        public static readonly Currency Eur = new("EUR", "€"); // Euro
        public static readonly Currency Gbp = new("GBP", "£"); // British Pound
        public static readonly Currency Jpy = new("JPY", "¥"); // Japanese Yen
        public static readonly Currency Aud = new("AUD", "A$"); // Australian Dollar
        public static readonly Currency Cad = new("CAD", "C$"); // Canadian Dollar
        public static readonly Currency Chf = new("CHF", "CHF"); // Swiss Franc
        public static readonly Currency Cny = new("CNY", "¥"); // Chinese Yuan Renminbi
        public static readonly Currency Inr = new("INR", "₹"); // Indian Rupee
        public static readonly Currency Rub = new("RUB", "₽"); // Russian Ruble
        
        // -- Other currencies --
        public static readonly Currency Aed = new("AED", "د.إ"); // UAE Dirham
        public static readonly Currency Ars = new("ARS", "$"); // Argentine Peso
        public static readonly Currency Bdt = new("BDT", "৳"); // Bangladeshi Taka
        public static readonly Currency Bgn = new("BGN", "лв"); // Bulgarian Lev
        public static readonly Currency Brl = new("BRL", "R$"); // Brazilian Real
        public static readonly Currency Btc = new("BTC", "₿"); // Bitcoin
        public static readonly Currency Clp = new("CLP", "$"); // Chilean Peso
        public static readonly Currency Cop = new("COP", "$"); // Colombian Peso
        public static readonly Currency Czk = new("CZK", "Kč"); // Czech Koruna
        public static readonly Currency Dkk = new("DKK", "kr"); // Danish Krone
        public static readonly Currency Egp = new("EGP", "E£"); // Egyptian Pound
        public static readonly Currency Eth = new("ETH", "Ξ"); // Ether
        public static readonly Currency Fjd = new("FJD", "FJ$"); // Fiji Dollar
        public static readonly Currency Ghs = new("GHS", "₵"); // Ghanaian Cedi
        public static readonly Currency Hkd = new("HKD", "HK$"); // Hong Kong Dollar
        public static readonly Currency Huf = new("HUF", "Ft"); // Hungarian Forint
        public static readonly Currency Idr = new("IDR", "Rp"); // Indonesian Rupiah
        public static readonly Currency Ils = new("ILS", "₪"); // Israeli New Shekel
        public static readonly Currency Iqd = new("IQD", "ع.د"); // Iraqi Dinar
        public static readonly Currency Kes = new("KES", "KSh"); // Kenyan Shilling
        public static readonly Currency Krw = new("KRW", "₩"); // South Korean Won
        public static readonly Currency Kwd = new("KWD", "د.ك"); // Kuwaiti Dinar
        public static readonly Currency Mxn = new("MXN", "$"); // Mexican Peso
        public static readonly Currency Myr = new("MYR", "RM"); // Malaysian Ringgit
        public static readonly Currency Nok = new("NOK", "kr"); // Norwegian Krone
        public static readonly Currency Ngn = new("NGN", "₦"); // Nigerian Naira
        public static readonly Currency Nzd = new("NZD", "NZ$"); // New Zealand Dollar
        public static readonly Currency Omr = new("OMR", "﷼"); // Omani Rial
        public static readonly Currency Php = new("PHP", "₱"); // Philippine Peso
        public static readonly Currency Pln = new("PLN", "zł"); // Polish Zloty
        public static readonly Currency Pkr = new("PKR", "₨"); // Pakistani Rupee
        public static readonly Currency Pyg = new("PYG", "₲"); // Paraguayan Guarani
        public static readonly Currency Ron = new("RON", "lei"); // Romanian Leu
        public static readonly Currency Rsd = new("RSD", "rsd."); // Serbian Dinar
        public static readonly Currency Sar = new("SAR", "﷼"); // Saudi Riyal
        public static readonly Currency Sek = new("SEK", "kr"); // Swedish Krona
        public static readonly Currency Sgd = new("SGD", "S$"); // Singapore Dollar
        public static readonly Currency Thb = new("THB", "฿"); // Thai Baht
        public static readonly Currency Try = new("TRY", "₺"); // Turkish Lira
        public static readonly Currency Twd = new("TWD", "NT$"); // Taiwan Dollar
        public static readonly Currency Vef = new("VEF", "Bs"); // Venezuelan Bolívar
        public static readonly Currency Vnd = new("VND", "₫"); // Vietnamese Dong
        public static readonly Currency Wst = new("WST", "WS$"); // Samoan Tala
        public static readonly Currency Zar = new("ZAR", "R"); // South African Rand
        
        public static readonly Currency DefaultCurrency = Usd;
        
        public static readonly IDictionary<string, Currency> All = new Dictionary<string, Currency>
        {
            { Usd.Code, Usd }, { Eur.Code, Eur }, { Gbp.Code, Gbp }, { Jpy.Code, Jpy },
            { Aud.Code, Aud }, { Cad.Code, Cad }, { Chf.Code, Chf }, { Cny.Code, Cny },
            { Inr.Code, Inr }, { Rub.Code, Rub },
            
            { Aed.Code, Aed }, { Ars.Code, Ars }, { Bdt.Code, Bdt }, { Bgn.Code, Bgn }, 
            { Brl.Code, Brl }, { Btc.Code, Btc }, { Clp.Code, Clp }, { Cop.Code, Cop },
            { Czk.Code, Czk }, { Dkk.Code, Dkk }, { Egp.Code, Egp }, { Eth.Code, Eth },
            { Fjd.Code, Fjd }, { Ghs.Code, Ghs }, { Hkd.Code, Hkd }, { Huf.Code, Huf }, 
            { Idr.Code, Idr }, { Ils.Code, Ils }, { Iqd.Code, Iqd }, { Kes.Code, Kes },
            { Krw.Code, Krw }, { Kwd.Code, Kwd }, { Mxn.Code, Mxn }, { Myr.Code, Myr },
            { Nok.Code, Nok }, { Ngn.Code, Ngn }, { Nzd.Code, Nzd }, { Omr.Code, Omr },
            { Php.Code, Php }, { Pln.Code, Pln }, { Pkr.Code, Pkr }, { Pyg.Code, Pyg },
            { Ron.Code, Ron }, { Rsd.Code, Rsd }, { Sar.Code, Sar }, { Sek.Code, Sek }, 
            { Sgd.Code, Sgd }, { Thb.Code, Thb }, { Try.Code, Try }, { Twd.Code, Twd }, 
            { Vef.Code, Vef }, { Vnd.Code, Vnd }, { Wst.Code, Wst }, { Zar.Code, Zar }
        };
    }
}