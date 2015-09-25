exec("./data.cs");
exec("./env.cs");
exec("./support.cs");
exec("./maps.cs");
exec("./saving.cs");
exec("./ranks.cs");

if(!$Haste::Init) {
	$Haste::Init = 1;
	schedule(1, 0, loadMapPhase1);
}

// !! ACM City needs spawns

schedule(1000, 0, fixEnv);

$loadOffset = "0 0 15";

function fixEnv() {
	setEnvironment("WaterHeight", 1);
	setEnvironment("WaterIdx", 2);
	setEnvironment("WaterColor", "0 0 0");
	setEnvironment("SkyColor", "128 192 255");
	setEnvironment("DirectLightColor", "192 192 192");
	setEnvironment("AmbientLightColor", "128 128 128");
	setEnvironment("ShadowColor", "80 80 80");
	setEnvironment("FogColor", "192 224 255");
}

function getAmountToRemove() {
	if(BrickGroup_888888.getCount() <= 0) {
		return -1;
	}
	return mCeil(BrickGroup_888888.getCount()*0.0315);
}

function startRemovingBricks() {
	$Haste::Running = 1;

	%toRemove = getAmountToRemove();
	if(%toRemove < 0) {
		return;
	}

	updateHasteAudio(1);
	$DefaultMinigame.playSound("hasteStartSound");

	$DefaultMinigame.respawnTime = -1000;

	$Haste::StartedAt = getSimTime();
	$Haste::InitialBrickCount = BrickGroup_888888.getCount();
	$Haste::RemoveSched = schedule(1, 0, removeBricks, %toRemove, 1);
}

function removeBricks(%amount, %new) {
	if(%amount < 0 || !$Haste::Running) {
		return;
	}
	if(%new) {
		newRemoveIteration(%amount);
	}

	if(%amount > 0) {
		%max = BrickGroup_888888.getCount()-1;
		%selection = getRandom(0, %max);
		%obj = BrickGroup_888888.getObject(%selection);
		if(!isObject(%obj)) {
			return;
		}

		%infLoopDetection = getRealTime();
		while(%obj.isSelected) {
			if(getRealTime() - %infLoopDetection >= 100) {
				//talk("Failed iteration at" SPC %amount SPC "remaining. resting for 100ms.");
				$Haste::DigSched = schedule(100, 0, removeBricks, %amount, 0);
				return;
			}
			%selection = getRandom(0, %max);
			%obj = BrickGroup_888888.getObject(%selection);
		}

		%obj.selectBrick(0);

		%map = MapDB.getObject($Haste::PreviousMap);
		$Haste::DigSched = schedule(getRandom(0,1)+(%map.delay*15), 0, removeBricks, %amount-1, 0);
	} else {
		%toRemove = getAmountToRemove();

		if(%toRemove <= 0) {
			return;
		}

		if(getSimTime() - $Haste::StartedRemoving < 1000) {
			%delay = 1000 - (getSimTime() - $Haste::StartedRemoving);
			$Haste::RemoveSched = schedule(%delay, 0, removeBricks, %toRemove, 1);
		} else {
			schedule(%delay, 0, removeBricks, %toRemove, 1);
		}
	}
}

function newRemoveIteration(%amount) {
	if(!getRandom(0, 8)) {
		spawnAsteroid();
	}

	%mg = $DefaultMinigame;
	if(%mg.numMembers > 0) {
		for(%i=0;%i<%mg.numMembers;%i++) {
			if(isObject(%mg.member[%i].player)) {
				%count++;
			}
		}
		if(!%count && $Haste::Running) {
			messageAll('', "Error: \c6No players alive, restarting...");

			setEnvironment("SkyColor", "128 192 255");
			setEnvironment("DirectLightColor", "192 192 192");
			setEnvironment("AmbientLightColor", "128 128 128");
			setEnvironment("ShadowColor", "80 80 80");
			setEnvironment("FogColor", "192 224 255");

			$Haste::Running = 0;

			for(%i=0;%i<%mg.numMembers;%i++) {
				cancel(%mg.member[%i].bpSched);
				cancel(%mg.member[%i].bpSched);
			}
			%mg.saveHasteGame();

			schedule(7000, 0, loadMapPhase1);
			return;
		}
	}

	$Haste::StartedRemoving = getSimTime();
	//talk("Removing" SPC %amount SPC "bricks");

	// %shift = mFloor(255*(($Haste::InitialBrickCount - BrickGroup_888888.getCount())/$Haste::InitialBrickCount));
	%shift = mFloor(255*(BrickGroup_888888.getCount()/$Haste::InitialBrickCount));
	%skyColor = "255" SPC %shift SPC %shift;
	%altColor = "192" SPC %shift/1.5 SPC %shift/1.5;
	//%skyColor = %shift SPC "48 48";
	//%altColor = %shift SPC "0 0";

	setEnvironment("SkyColor", %skyColor);
	setEnvironment("DirectLightColor", %altColor);
	setEnvironment("AmbientLightColor", %altColor);
	setEnvironment("ShadowColor", %altColor);
	setEnvironment("FogColor", %altColor);
}

function spawnAsteroid() {
	if(BrickGroup_888888.getCount() <= 0) {
		return;
	}

	%proj = new Projectile(HasteAsteroid) {
		scale = "2 2 2";
		dataBlock = HasteAsteroidProjectile;
		initialVelocity = "0 0" SPC getRandom(-60,-40);
		initialPosition = vectorAdd(BrickGroup_888888.getObject(getRandom(0, BrickGroup_888888.getCount()-1)).getPosition(), "0 0" SPC getRandom(70, 150));
		sourceObject = -1;
		sourceSlot = 0;
		client = -1;
	};
	MissionCleanup.add(%proj);
}