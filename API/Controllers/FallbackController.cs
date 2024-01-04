using Microsoft.AspNetCore.Mvc;

namespace API;

public class FallbackController : Controller
{

    public ActionResult Index() {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","index.html"),"text/HTML");

    }

}
