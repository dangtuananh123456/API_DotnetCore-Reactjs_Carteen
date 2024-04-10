namespace IntroSEProject.API.Middlewares
{
    public class ReadAccessTokenFromCookieMiddleware
    {
        private readonly RequestDelegate next;

        public ReadAccessTokenFromCookieMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var cookie = context.Request.Cookies["access_token"];
            if (cookie != null)
            {
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Request.Headers.Append("Authorization", $"Bearer {cookie}");
                }
            }
            await next.Invoke(context);
        }
    }
}
