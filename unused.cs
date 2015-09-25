// !! abandoned, can just use a setting variable for each build
function startDominantColorDig() {
	talk("Getting color for the round...");
	%colors = 0;

	while(isObject("color" @ %colors @ "SprayCanImage")) {
		$Haste::Colors[%colors] = 0;
		%colors++;
	}

	$Haste::DominantColorDig = schedule(1, 0, dominantColorDig, 0);
}
function dominantColorDig(%id) {
	if(%id == (BrickGroup_888888.getCount() - 1) || %id >= 1000) {
		%colors = 0;
		while(isObject("color" @ %colors @ "SprayCanImage")) {
			talk("Found" SPC $Haste::Colors[%colors] SPC "of" SPC %colors);
			%colors++;
		}
		for(%i=0;%i<%colors;%i++) {
			%data = getColorIDTable(%i);
			if(getWord(%data, 3) < 1) {
				// transparent colors are glitchy, let's not
				continue;
			}
			if($Haste::Colors[%i] == 0) {
				%chosen = %i;
				break;
			}
		}

		return;
	}
	echo("Looking at" SPC %id);

	%obj = BrickGroup_888888.getObject(%id);
	%col = %obj.colorID;

	$Haste::Colors[%col]++;

	$Haste::DominantColorDig = schedule(1, 0, dominantColorDig, %id+1);
}