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
    let cfg: Config = load_and_validate_config();

    let selection = get_selection();
    
    println!("You chose {}", selection);
}

fn validate_config_field(field: &String, file_name: String) -> String {    
    let mut output = field.to_string();

    while !Path::new(&output).is_file() {
        println!("Enter the path to your {} file.", file_name); 
        
        io::stdin().read_line(&mut output).expect("Failed to read a line");
        output = output.trim().to_string();
    };

    return output;
}

fn load_and_validate_config() -> Config {
    let cfg = match confy::load_path("./app_config.toml") {
        Ok(val) => val,
        Err(msg) => {
            println!("\r\nMalformed configuration file with the error:");
            println!("\t{}\r\n", msg);
            println!("Creating a new file! Backup your old file now if you don't want to lose any changes.\r\n");
            Config::default()
        }
    };

    // Check if config is valid
    // Validate config_path. If it is not valid, ask for one until there is a valid response
    let new_config = validate_config_field(&cfg.config_path, "FFXIV.cfg".to_string());
    let new_exe = validate_config_field(&cfg.exe_path, "ffxivboot.exe".to_string());

    if new_config != cfg.config_path || new_exe != cfg.exe_path {
        let cfg = Config { config_path: new_config, exe_path: new_exe };
        confy::store_path("./app_config.toml", &cfg).expect("Failed to save config");
    }

    return cfg;
}

fn get_selection() -> i16 {
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

    return selection;
}