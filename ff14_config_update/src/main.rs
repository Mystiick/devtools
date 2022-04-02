// Standard
use std::io;
use std::io::Error;
use std::process::Command;

// Crate
mod app_config;
mod ffxiv_config;
use app_config::AppConfig;


fn main() -> Result<(), Error> {
    // Load config
    let cfg: AppConfig = AppConfig::load_from_file("./app_config.toml")?;

    // Get user input
    let selection = get_selection()?;

    // Update config file if needed
    if selection >= 0 {
        ffxiv_config::update_file(&cfg, selection)?;
    }
    
    // Run .exe
    Command::new(&cfg.exe_path).spawn()?;

    Ok(())
}

fn get_selection() -> Result<i16, Error> {
    let mut selection: i16 = -1;
    let mut input = String::new();

    while selection == -1 {
        println!("0) Fullscreen - Right Window");
        println!("1) Windowed - Right Window");
        println!("2) Windowed - Left Window");
    
        println!("Enter a choice:");
        io::stdin().read_line(&mut input)?;

        if input.trim().is_empty() {
            println!("Launching with prior settings.");
            return Ok(-1);
        }

        selection = match input.trim().parse() {
            Ok(val) => val,
            Err(_) => continue
        };
    }

    return Ok(selection);
}
