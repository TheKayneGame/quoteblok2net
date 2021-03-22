using System;

namespace quoteblok2net.Utilities.Configs
{

    public class Config
    {
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

        public class MongoDB {
            public String pass;
            public String login;
            public String address;
            public String database;
        }

        public String token;
        public MongoDB mongoDB;
        public String defaultPrefix;
        


    }
}
