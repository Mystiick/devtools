use confy;
use serde::{Serialize, Deserialize};
use std::io;
use std::path::Path;

#[derive(Debug, Serialize, Deserialize)]
struct Config {
    config_path: String,
    exe_path: String
}
impl Default for Config {
    fn default() -> Self {
        Config {
            config_path: "".to_string(),
            exe_path: "".to_string(),
        }
    }
}

fn main() {
    // Load config
    let mut cfg: Config = confy::load_path("./app_config.toml").expect("Failed load load config");
    
    // Check if config is valid
    // Validate config_path. If it is not valid, ask for one until there is a valid response
    while !Path::new(&cfg.config_path).is_file() {
        println!("Invalid path: {}", &cfg.config_path);
        println!("Enter the path to your FFXIV.cfg file. (My Documents/My Games on Windows)"); 
        
        let mut file_path = String::new();
        io::stdin().read_line(&mut file_path).expect("Failed to read a line");

        cfg = Config {
            config_path: file_path.trim().to_string(),
            ..cfg
        };
        
        confy::store_path("./app_config.toml", &cfg).expect("Failed to save config");
    };

    // Validate config_path. If it is not valid, ask for one until there is a valid response
    while !Path::new(&cfg.exe_path).is_file() {
        println!("Invalid path: {}", &cfg.exe_path);
        println!("Enter the path to your ffxivboot.exe file. (In your install directory)"); 
        
        let mut file_path = String::new();
        io::stdin().read_line(&mut file_path).expect("Failed to read a line");

        cfg = Config {
            exe_path: file_path.trim().to_string(),
            ..cfg
        };
        
        confy::store_path("./app_config.toml", &cfg).expect("Failed to save config");
    };

    let mut selection: i16 = -1;
    let mut input = String::new();

    while selection == -1 {
        println!("0) Fullscreen - Right Window");
        println!("1) Windowed - Right Window");
        println!("2) Windowed - Left Window");
    
        println!("Enter a choice:");
        io::stdin().read_line(&mut input).expect("Failed to read input");

        selection = match input.trim().parse() {
            Ok(val) => val,
            Err(_) => continue
        };
        
    }

    println!("You chose {}", selection);
}

