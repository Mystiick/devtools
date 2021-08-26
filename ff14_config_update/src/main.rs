use confy;
use serde::{Serialize, Deserialize};

use std::fs::File;
use std::io;
use std::io::prelude::*;
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
    let cfg: Config = load_and_validate_config();
    let selection = get_selection();

    update_file(&cfg, selection);
    launch_game(&cfg);
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

fn validate_config_field(field: &String, file_name: String) -> String {
    let mut output = field.to_string();

    while !Path::new(&output).is_file() {
        println!("Enter the path to your {} file.", file_name); 
        
        io::stdin().read_line(&mut output).expect("Failed to read a line");
        output = output.trim().to_string();
    };

    return output;
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

// TODO: Should probably return Result?
fn update_file(cfg: &Config, selection: i16) {

    println!("You chose {}", selection);

    // TODO: Put this in a config or something
    let screen_left: &str = match selection {
        0 => "0",           // Fullscreen right
        1 => "0",           // Windowed right
        2 => "4294965376",  // Windowed left
        _ => panic!("Invalid selection!")
    };
    let screen_mode: &str = match selection {
        0 => "2",
        1 => "0",
        2 => "0",
        _ => panic!("Invalid selection!")
    };

    // Read the file
    let path = Path::new(&cfg.config_path);

    let mut file = match File::open(&path) {
        Ok(val) => val,
        Err(msg) => panic!("Failed to open file {}. {}.", path.display(), msg)
    };

    let mut file_text = String::new();
    match file.read_to_string(&mut file_text) {
        Ok(_) => println!("File successfully read"),
        Err(msg) => panic!("{}", msg)
    }
    
    let mut new_file_text = file_text.to_string();
    // Loop over each line, and look for the configs we want to update
    for line in file_text.split('\n') {
        if line.starts_with("ScreenLeft") {
            let new_line = update_line(&line, screen_left);
            new_file_text = new_file_text.replace(line, &new_line);
        }
        else if line.starts_with("ScreenMode") {
            let new_line = update_line(&line, screen_mode);
            new_file_text = new_file_text.replace(line, &new_line);
        }
    }

    // Save the new file
    // TODO: Better error handling
    match file.write_all(new_file_text.as_bytes()) {
        Ok(_) => return,
        Err(msg) => panic!("{}", msg)
    }
}

fn update_line(line: &str, new_val: &str) -> String {
    let split: Vec<&str> = line.split_whitespace().collect();
    let val = split[1];
    
    return line.replace(&val, new_val);
}

fn launch_game(cfg: &Config) {
    println!("{}", cfg.exe_path);
}