A collection of misc scripts and console apps written by me

## GachaCalc
`C#`: Simple application to calculate how many summons or drops are needed with a guaranteed drop mechanic.

## MicrophoneMute
`C#`: Console app to mute/unmute connected microphones on Windows 10. Uses NAudio to do the heavy lifting.

## SaveBackup
`C#`: Console app to backup files from one directory to another optionally git controlled folder

## ff14_config_update
`rust`: Console app that updates the configuration for Final Fantasy 14 to determine which monitor and windowed status it should open with.

## better-twitch-theatre
`js`: A small script for twitch that will place the chatbox below the video player. Twitch has functionality built in for tall windows, but only up to 918px. This script allows you to always have the chatbox below the player, regardless of screen size. NOTE: the video player is 100% width, so if you're viewing fullscreen on a 16:9 horizontal monitor, the chatbox will just disappear below the player. The window needs to be taller than it is wide.

To use: Create a new bookmark in your browser. Copy the contents of the [.js file](better-twitch-theatre.js) as the URL of the bookmark (for Chrome, you need to remove the line-breaks manually. Firefox does this for you). Open your favorite stream in a vertical window and click the bookmark. Alternatively you can just copy/paste the contents of the file (without `javascript:`) into the browser's console to run it. There are probably addons that let you do this another way as well, pick your poison.

If the script stops working, Twitch may have updated their styling, and you'll need a new version of the script. Check back for an updated script, I typically have a stream going all the time, so I'll notice if the styles change and fix it for myself. `class_beside` and `class_below` changes anytime Twitch deploys a new minified version of their CSS (I assume), so they need to be updated manually  ¯\\\_(ツ)\_/¯

## blind-commit
`batch`: A Windows batch script that is used to blindly commit and push all files (git add .) to the repository.

## git-loop
`bash`: A bash script that can be used to delete dead branches locally, or find un-pushed commits.
