onmessage = function(event) {
    setInterval(function(){
        postMessage('tick');
    },event.data.ms || 0);
};