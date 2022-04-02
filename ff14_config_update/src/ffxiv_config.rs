// Standard
use std::io::Error;
use std::fs::OpenOptions;
use std::io::prelude::*;
use std::path::Path;

// Crate
use crate::app_config::AppConfig;

pub fn update_file(cfg: &AppConfig, selection: i16) -> Result<(), Error> {

    println!("You chose {}", selection);

    // TODO: Put this in a config or something
    let screen_left: &str = match selection {
        0 => "0",           // Fullscreen right
        1 => "0",           // Windowed right
        2 => "4294966216",  // Windowed left
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

    let mut file = OpenOptions::new()
        .read(true)
        .write(true)
        .open(path)?;

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
    file.seek(std::io::SeekFrom::Start(0))?;
    file.write_all(new_file_text.as_bytes())?;

    Ok(())
}

pub fn update_line(line: &str, new_val: &str) -> String {
    let split: Vec<&str> = line.split_whitespace().collect();
    let val = split[1];
    
    return line.replace(&val, new_val);
}
