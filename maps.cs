function initMapDB() {
	if(isObject(MapDB)) {
		%count = MapDB.getCount();
		for(%i=0;%i<%count;%i++) {
			MapDB.getObject(0).delete();
		}
	} else {
		new SimSet(MapDB);
	}

	%pattern = "Add-Ons/Instability_*/*.bls";
	%file = findFirstFile(%pattern);

	while(isFile(%file)) {
		echo("Found" SPC %file);
		%path = filePath(%file);

		if(!isFile(%path @ "/info.txt")) {
			warn("No info file found, skipping...");
			%file = findNextFile(%pattern);
			continue;
		}

		%obj = new FileObject();
		%obj.openForRead(%path @ "/info.txt");
		
		%title = %obj.readLine();
		%author = %obj.readLine();
		%delay = %obj.readLine();

		%obj.close();
		%obj.delete();

		%obj = new ScriptObject(HasteMapData) {
			title = %title;
			author = %author;
			delay = %delay;
			file = %file;
		};
		MapDB.add(%obj);

		echo("Added" SPC %title);
		
		%file = findNextFile(%pattern);
	}
}