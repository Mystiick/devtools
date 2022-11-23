javascript: (() => {
    var class_beside = "fqTSIB";
    var class_below = "dhPeII";

    /* Open theatre mode */
    var t = document.querySelector('[aria-label="Theatre Mode (alt+t)"]');
    if (t) t.click();

    /* Change chatbox to below */
    var chat = document.getElementsByClassName("right-column--theatre")[0];
    document.getElementsByClassName("persistent-player--theatre")[0].style = "top: 0px; left: 0px; position: fixed; z-index: 3000; height: 56.2vw; transform: scale(1); width: 100vw;";
    chat.className = chat.className.replace("right-column--beside", "right-column--below").replace(class_beside, class_below);

    /* Filter through the stylesheets, find the core sheet and add position styles */
    var c = Array.prototype.filter.call(
        Array.prototype.filter.call(
            document.styleSheets,
            x => x.href && x.href.indexOf("assets/core") != -1 /*"url like https://static.twitchcdn.net/assets/core-57b1d4b350873f588661.css"*/
        )[0].cssRules,
        y => y.selectorText && y.selectorText.indexOf("right-column--below") != -1
    )[0];
    c.style["position"] = "absolute";
    c.style["left"] = "0";
    c.style["bottom"] = "0";

    /* Remove the top gifter/bits */
    document.getElementsByClassName("channel-leaderboard-marquee")[0].style = "display:none";
    document.getElementsByClassName("stream-chat-header")[0].style = "display:none";
})();