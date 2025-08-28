namespace ClassLibrary1
{
    public static class SD
    {
      public  enum ApiType
        {
            Get,
            Post,
            Put,
            Delete
        }
        public static string AccessToken = "JWTSession1144";
        public static string CurrentVersion = "v2";
    }

    public enum ContentType
    {
        Json,
        Fille
    }
}
