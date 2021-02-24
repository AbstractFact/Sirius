var Singleton = (function () {
    var instance;
 
    function createInstance() {
        var object = new Object("I am the instance");
        console.log("aaa");
        return object;
    }
 
    return {
        getInstance: function () {
            if (!instance) {
                instance = createInstance();
            }
            return instance;
        }
    };
})();
 
export default Singleton;