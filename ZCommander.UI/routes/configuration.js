var express = require('express');
var router = express.Router();

router.get('/', function(req, res) {
  res.render('configuration/index', { title: 'Configuration' });
});



/* GET task listing. */
router.get('/tasks/tasklist', function(req, res) {
    var db = req.db;
    var collection = db.get('taskcollection');
    collection.find({},{},function(e,docs){
        res.render('configuration/tasks/tasklist', {
            "tasklist" : docs
        });
    });
});

/* GET New Task page. */
router.get('/tasks/newtask', function(req, res) {
    res.render('configuration/tasks/newtask', { title: 'Add New Task' });
});

/* GET Edit Task page. */
router.get('/tasks/edittask/:id', function(req, res) {
    var db = req.db;
    var collection = db.get('taskcollection');
    collection.findOne({_id: req.param("id")},{},function(e,doc){
        res.render('configuration/tasks/edittask', {
            "task" : doc
        });
    });
});

/* POST to Add Task Service */
router.post('/tasks/savetask/:id', function(req, res) {
    var db = req.db;
    var name = req.body.name;
    var description = req.body.description;
    var configuration = req.body.configuration;
    var type = req.body.type;
    var collection = db.get('taskcollection');

    collection.update(
        {_id:req.param("id")},
        {
            "name" : name,
            "description" : description,
            "configuration" : configuration,
            "type" : type
        }, 
        function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem saving the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("/configuration");
            // And forward to success page
            res.redirect("/configuration/tasks/tasklist");
        }
    });
});

/* GET factory listing. */
router.get('/factories/factorylist', function(req, res) {
    var db = req.db;
    var collection = db.get('factorycollection');
    collection.find({},{},function(e,docs){
        res.render('configuration/factories/factorylist', {
            "factorylist" : docs
        });
    });
});

/* GET New factory page. */
router.get('/factories/newfactory', function(req, res) {
    res.render('configuration/factories/newfactory', { title: 'Add New factory' });
});

/* GET Edit factory page. */
router.get('/factories/editfactory/:id', function(req, res) {
    var db = req.db;
    var collection = db.get('factorycollection');
    collection.findOne({_id: req.param("id")},{},function(e,doc){
        res.render('configuration/factories/editfactory', {
            "factory" : doc
        });
    });
});

/* GET Edit factory page. */
router.post('/factories/deletefactory/:id', function(req, res) {
    var db = req.db;
    var collection = db.get('factorycollection');
    collection.remove({_id: req.param("id")},{},function(e,doc){
        res.render('configuration/factories/editfactory', {
            "factory" : doc
        });
    });
});

/* POST to Add factory Service */
router.post('/factories/savefactory/:id', function(req, res) {
    var db = req.db;
    var name = req.body.name;
    var description = req.body.description;
    var configuration = req.body.configuration;
    var type = req.body.type;
    var collection = db.get('factorycollection');

    collection.update(
        {_id:req.param("id")},
        {
            "name" : name,
            "description" : description,
            "configuration" : configuration,
            "type" : type
        }, 
        function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem saving the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("/configuration");
            // And forward to success page
            res.redirect("/configuration/factories/factorylist");
        }
    });
});

/* POST to Add factory Service */
router.post('/factories/savefactory', function(req, res) {
    var db = req.db;
    var name = req.body.name;
    var description = req.body.description;
    var configuration = req.body.configuration;
    var type = req.body.type;
    var collection = db.get('factorycollection');

    collection.insert(
        {
            "name" : name,
            "description" : description,
            "configuration" : configuration,
            "type" : type
        }, 
        function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem saving the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("/configuration");
            // And forward to success page
            res.redirect("/configuration/factories/factorylist");
        }
    });
});



/* GET assembly listing. */
router.get('/assemblies/assemblylist', function(req, res) {
    var db = req.db;
    var collection = db.get('assemblycollection');
    collection.find({},{},function(e,docs){
        res.render('configuration/assemblies/assemblylist', {
            "assemblylist" : docs
        });
    });
});

/* GET New assembly page. */
router.get('/assemblies/newassembly', function(req, res) {
    res.render('configuration/assemblies/newassembly', { title: 'Add New assembly' });
});

/* GET Edit assembly page. */
router.get('/assemblies/editassembly/:id', function(req, res) {
    var db = req.db;
    var collection = db.get('assemblycollection');
    collection.findOne({_id: req.param("id")},{},function(e,doc){
        res.render('configuration/assemblies/editassembly', {
            "assembly" : doc
        });
    });
});

/* GET Edit assembly page. */
router.post('/assemblies/deleteassembly/:id', function(req, res) {
    var db = req.db;
    var collection = db.get('assemblycollection');
    collection.remove({_id: req.param("id")},{},function(e,doc){
        res.render('configuration/assemblies/editassembly', {
            "assembly" : doc
        });
    });
});

/* POST to Add assembly Service */
router.post('/assemblies/saveassembly/:id', function(req, res) {
    var db = req.db;
    var name = req.body.name;
    var description = req.body.description;
    var configuration = req.body.configuration;
    var collection = db.get('assemblycollection');

    collection.update(
        {_id:req.param("id")},
        {
            "name" : name,
            "description" : description,
            "configuration" : configuration
        }, 
        function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem saving the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("/configuration");
            // And forward to success page
            res.redirect("/configuration/assemblies/assemblylist");
        }
    });
});

/* POST to Add assembly Service */
router.post('/assemblies/saveassembly', function(req, res) {
    var db = req.db;
    var name = req.body.name;
    var description = req.body.description;
    var configuration = req.body.configuration;
    var collection = db.get('assemblycollection');

    collection.insert(
        {
            "name" : name,
            "description" : description,
            "configuration" : configuration
        }, 
        function (err, doc) {
        if (err) {
            // If it failed, return error
            res.send("There was a problem saving the information to the database.");
        }
        else {
            // If it worked, set the header so the address bar doesn't still say /adduser
            res.location("/configuration");
            // And forward to success page
            res.redirect("/configuration/assemblies/assemblylist");
        }
    });
});

module.exports = router;