using System;

namespace quoteblok2net.BotSystem.Configuration
{

    public class Config
    {
        public class MongoDB
        {
            public String pass;
            public String login;
            public String address;
            public String database;
        }
        public class CachingExpiration
        {
            public int RoleMenu;
            public int ServerSettings;
        }

        public Config(){
            token = "DISCORD_TOKEN";
            mongoDB = new MongoDB() {pass = "", login = "", address = "", database = ""};
            defaultPrefix = "PREFIX";
        }
        public Config(Config config){
            token = config.token;
            mongoDB = config.mongoDB;
            defaultPrefix = config.defaultPrefix;
        }

        public String token;
        public MongoDB mongoDB;
        public String defaultPrefix;

        public CachingExpiration cachingExpiration;



    }
}
