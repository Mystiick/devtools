// Standard
use std::io;
use std::io::Error;
use std::path::Path;

// Lib
use confy;
use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct AppConfig {
    pub config_path: String,
    pub exe_path: String
}
impl Default for AppConfig {
    fn default() -> Self {
        AppConfig {
            config_path: "".to_string(),
            exe_path: "".to_string(),
        }
    }
}
impl AppConfig {
    pub fn load_from_file(path: &str) -> Result<AppConfig, Error> {

        let output = match confy::load_path(path) {
            Ok(val) => val,
            Err(msg) => {
                println!("\r\nMalformed configuration file with the error:");
                println!("\t{}\r\n", msg);
                println!("Creating a new file! Backup your old file now if you don't want to lose any changes.\r\n");
                AppConfig::default()
            }
        };

        Ok(validate_config(output)?)
    }
}

pub fn validate_config(cfg: AppConfig) -> Result<AppConfig, Error> {
    // Check if config is valid
    // Validate config_path. If it is not valid, ask for one until there is a valid response
    let new_config = validate_config_field(&cfg.config_path, "FFXIV.cfg".to_string())?;
    let new_exe = validate_config_field(&cfg.exe_path, "ffxivboot.exe".to_string())?;

    if new_config != cfg.config_path || new_exe != cfg.exe_path {
        let cfg = AppConfig { config_path: new_config, exe_path: new_exe };
        confy::store_path("./app_config.toml", &cfg).expect("Failed to save config");
    }

    return Ok(cfg);
}

fn validate_config_field(field: &String, file_name: String) -> Result<String, Error> {
    let mut output = field.to_string();

    while !Path::new(&output).is_file() {
        println!("Enter the path to your {} file.", file_name); 
        
        io::stdin().read_line(&mut output)?;
        output = output.trim().to_string();
    };

    return Ok(output);
}