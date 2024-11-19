using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.UseMiddleware<LoggingMiddleware>();

app.Map("/", HomePageHandler);
app.Map("/upload", UploadPageHandler);
app.Map("/gallery", GalleryPageHandler);

app.Run();

async Task HomePageHandler(HttpContext context)
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(@"
    <html>
    <head>
        <title>����-�������</title>
        <meta charset='UTF-8'>
    </head>
    <body>
        <h1>����� ���������� � �������!</h1>
        <p><a href='/upload'>��������� �����������</a></p>
        <p><a href='/gallery'>����������� �������</a></p>
    </body>
    </html>
    ");
}

async Task UploadPageHandler(HttpContext context)
{
    if (context.Request.Method == "GET")
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(@"
        <html>
        <head>
            <title>�������� �����������</title>
            <meta charset='UTF-8'>
        </head>
        <body>
            <h1>�������� �����������</h1>
            <form method='post' enctype='multipart/form-data'>
                <input type='file' name='image' accept='image/*' required />
                <button type='submit'>���������</button>
            </form>
        </body>
        </html>
        ");
    }
    else if (context.Request.Method == "POST")
    {
        var form = await context.Request.ReadFormAsync();
        var file = form.Files["image"];

        if (file != null && file.Length > 0)
        {
            var fileName = System.IO.Path.GetFileName(file.FileName);
            var savePath = $"wwwroot/images/{fileName}";

            using (var stream = System.IO.File.Create(savePath))
            {
                await file.CopyToAsync(stream);
            }

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync($@"
            <html>
            <head>
                <title>�������� �����������</title>
                <meta charset='UTF-8'>
            </head>
            <body>
                <h1>����������� ������� ���������!</h1>
                <p><a href='/gallery'>������� � �������</a></p>
            </body>
            </html>
            ");
        }
        else
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync($@"
            <html>
            <head>
                <title>������ ��������</title>
                <meta charset='UTF-8'>
            </head>
            <body>
                <h1>������ ��� �������� �����������.</h1>
                <p><a href='/upload'>����������� �����</a></p>
            </body>
            </html>
            ");
        }
    }
}

async Task GalleryPageHandler(HttpContext context)
{
    var imagesPath = "wwwroot/images";
    var imageFiles = System.IO.Directory.GetFiles(imagesPath)
        .Select(f => "/images/" + System.IO.Path.GetFileName(f))
        .ToList();

    context.Response.ContentType = "text/html; charset=utf-8";

    var html = @"
    <html>
    <head>
        <title>������� �����������</title>
        <meta charset='UTF-8'>
        <link rel='stylesheet' href='/css/styles.css'>
        <link rel='stylesheet' href='/lightbox/css/lightbox.css'>
    </head>
    <body>
        <h1>������� �����������</h1>
        <div class='gallery'>";

    foreach (var image in imageFiles)
    {
        html += $"<a href='{image}' data-lightbox='gallery'><img src='{image}' alt='�����������' /></a>";
    }

    html += @"
        </div>
        <p><a href='/'>�� �������</a></p>
        <script src='/lightbox/js/lightbox-plus-jquery.js'></script>
    </body>
    </html>";

    await context.Response.WriteAsync(html);
}
