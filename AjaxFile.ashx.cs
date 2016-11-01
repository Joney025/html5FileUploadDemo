using System;
using System.IO;
using System.Web;

namespace html5FileUploadDemo
{
    /// <summary>
    /// AjaxFile 的摘要说明
    /// </summary>
    public class AjaxFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var data = context.Request.Files["data"]; //slice方法用于切出文件的一部分
            var lastModified = context.Request.Form["lastModified"];
            var name = context.Request.Form["name"];
            var total = Convert.ToInt32(context.Request.Form["total"].ToString()); //总片数
            var index = Convert.ToInt32(context.Request.Form["index"].ToString());//当前是第几片

            string dir = context.Server.MapPath("~/Upload");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string file = Path.Combine(dir, name + "_" + index);

            data.SaveAs(file);

            //如果已经是最后一个分片，组合

            //当然你也可以用其它方法比如接收每个分片时直接写到最终文件的相应位置上，但要控制好并发防止文件锁冲突

            if (index == total)
            {
                file = Path.Combine(dir, name);

                var fs = new FileStream(file, FileMode.Create);

                for (int i = 1; i <= total; ++i)
                {
                    string part = Path.Combine(dir, name + "_" + i);

                    var bytes = System.IO.File.ReadAllBytes(part);

                    fs.Write(bytes, 0, bytes.Length);

                    bytes = null;
                    System.IO.File.Delete(part);
                }
                fs.Close();
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(index);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}