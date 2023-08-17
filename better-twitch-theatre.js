javascript: (() => {
    /* className: class of the object to hide | elementFinder: function to fetch the single element to hide */
    function hide(className, elementFinder) {
        try {
            elementFinder(document.getElementsByClassName(className)).style = "display:none !important"
        }
        catch { }
    }

    var class_beside = "fqTSIB"; /* class twitch assigns the chatbox for the right side */
    var class_below = "dhPeII"; /* class for the chat below */

    /* Open theatre mode */
    var t = document.querySelector('[aria-label="Theatre Mode (alt+t)"]');
    if (t) t.click();

    /* Change chatbox to below the stream */
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

    /*  Hide individual elements */
    hide("jaGgya", e => e[0].parentNode);  /* channel leaderboard */
    hide("stream-chat-header", e => e[0]); /* 'stream chat' header */
    hide("right-column__toggle-visibility", e => e[0]); /* expand arrow */
    hide("paid-pinned-chat-message-list", e => e[0]); /* paid chats */
    hide("Layout-sc-1xcs6mc-0 ivcMfs", e => e[0].parentElement.parentElement); /* more paid chats */
})();