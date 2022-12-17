using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace RestApi_MB_DovizKuru.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyExchangeRateController : ControllerBase
    {
        [HttpPost]
        public ResponseData Run(RequestData request)
        {
            ResponseData result = new ResponseData();
            try
            {
                string tcmblink = string.Format("https://www.tcmb.gov.tr/kurlar/{0}.xml", (request.IsToday) ? "today" : string.Format("{2}{1}/{0}{1}{2}", request.Day.ToString().PadLeft(2, '0'), request.Month.ToString().PadLeft(2, '0'), request.Year.ToString()));
                result.Data = new List<ResponseDataDovizKuru>();

                XmlDocument doc = new XmlDocument();

                doc.Load(tcmblink);
                if (doc.SelectNodes("Tarih_Date").Count<1)
                {
                    result.Error = "Currency information not found.";
                    return result;
                }
                foreach (XmlNode node in doc.SelectNodes("Tarih_Date")[0].ChildNodes)
                {
                    ResponseDataDovizKuru kur = new ResponseDataDovizKuru();

                    kur.Code = node.Attributes["Kod"].Value;
                    kur.Name = node["Isim"].InnerText;
                    kur.Unit = int.Parse(node["Unit"].InnerText);
                    kur.BuyingRate = Convert.ToDecimal("0" + node["ForexBuying"].InnerText.Replace(".", ","));
                    kur.SellingRate = Convert.ToDecimal("0" + node["ForexSelling"].InnerText.Replace(".", ","));
                    kur.EffectiveBuyingRate = Convert.ToDecimal("0" + node["BanknoteBuying"].InnerText.Replace(".", ","));
                    kur.EffectiveSellingRate = Convert.ToDecimal("0" + node["BanknoteSelling"].InnerText.Replace(".", ","));
                    result.Data.Add(kur);
                }

            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return result;
        }
    }
}
