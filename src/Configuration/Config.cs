using System;

namespace quoteblok2net.BotSystem
{

    public class Config
    {
        public Config(){
            token = "DISCORD_TOKEN";
            mongoDB = new MongoDB() {pass = "", login = "", address = "", database = ""};
        }
        public Config(Config config){
            token = config.token;
            mongoDB = config.mongoDB;
        }

        public class MongoDB {
            public String pass;
            public String login;
            public String address;
            public String database;
        }

        public String token;
        public MongoDB mongoDB;
        


    }
}
