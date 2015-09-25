function GameConnection::updateBottomPrint(%this) {
	cancel(%this.bpSched);
	%this.bpSched = %this.schedule(100, updateBottomPrint);

	%time = (getSimTime() - $Haste::StartedAt)/1000;
	if(%time < 0) {
		%time = %time*-1;
	}
	%this.bottomPrint("<font:Arial:16>\c3Time:\c6" SPC getTimeString(mFloor(%time)) @ "<just:right>\c3Wins:\c6" SPC %this.score @ "   \c3Bricks:\c6" SPC BrickGroup_888888.getCount(),15,1);
}

function loadMapPhase1() {
	BrickGroup_888888.chaindeletecallback = "loadMapPhase2(\"" @ getMap() @ "\");";
	BrickGroup_888888.chaindeleteall();
}
function loadMapPhase2(%filename) {
	%mg = $DefaultMinigame;

	for(%i=0;%i<%mg.numMembers;%i++) {
		%client = %mg.member[%i];
		%player = %client.player;

		if(isObject(%player))
			%player.delete();

		%camera = %client.camera;
		%camera.setFlyMode();
		%camera.mode = "Observer";
		%client.setControlObject(%camera);
	}

	%map = MapDB.getObject($Haste::PreviousMap);
	%mg.messageAll('', "\c5Loading\c3" SPC %map.title SPC "\c5by\c3" SPC %map.author);

	$LoadingBricks_BrickGroup = BrickGroup_888888;
	serverDirectSaveFileLoad(%filename, 3, "", 2, 0);
}

function getMap() {
	if(!isObject(MapDB)) {
		initMapDB();
	}

	%id = getRandom(0, MapDB.getCount()-1);
	while(%id == $Haste::PreviousMap) {
		%id = getRandom(0, MapDB.getCount()-1);
	}
	$Haste::PreviousMap = %id;

	return MapDB.getObject(%id).file;
}

function fxDTSBrick::selectBrick(%this, %activator) {
	if(%this.isSelected) {
		return -1;
	}

	%this.isSelected = 1;

	%this.setColorFX(3);
	if(%activator) {
		%this.playSound("hasteClickSound");
		if(%this.colorID != 7) {
			%this.setColor(7);
		} else {
			%this.setColor(0);
		}
	} else {
		%sound = "hastePlantSound" @ getRandom(1, 8);
		%this.playSound(%sound);
		if(%this.colorID != 3) {
			%this.setColor(3);
		} else {
			%this.setColor(0);
		}
	}

	%this.schedule(500, delete);
}

// http://forum.blockland.us/index.php?topic=235991.msg6716432#msg6716432
// >not using Player::
function Player::checkClickedObject(%pl)
{
	%eye = vectorScale(%pl.getEyeVector(), 10);
	%pos = %pl.getEyePoint();
	%mask = $TypeMasks::All;
	%hit = firstWord(containerRaycast(%pos, vectorAdd(%pos, %eye), %mask, %pl));
		
	if(!isObject(%hit)) {
		return;
	}

	if(vectorDist(%pl.getPosition(), %hit.getPosition()) < 4) {
		if(%hit.getClassName() $= "Player") {
			if(getSimTime() - %this.lastClickPush > 500 && $Haste::Running) {
				%hit.addVelocity(vectorScale(%pl.getEyeVector(), 3));
				%this.lastClickPush = getSimTime();
			}
			return;
		}
		if(%hit.getClassName() $= "fxDTSBrick") {
			if($Haste::Running) {
				%hit.selectBrick(1);
			}
		}
	}
}

function MinigameSO::playSound(%this, %databl) {
	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];
		%client.playSound(%databl);
	}
}

function HasteAsteroid::onCollision(%this,%obj,%col,%fade,%pos,%normal) {
	if(%col.getClassName() $= "fxDTSBrick") {
		InitContainerRadiusSearch(%pos,6,$TypeMasks::FXBrickObjectType);
		while((%targetObject = containerSearchNext()) != 0) {
			%targetObject.selectBrick(0);
		}
	}
}

package HastePackage {

	function GameConnection::autoAdminCheck(%this) {
		%this.updateBottomPrint();
		%this.origPrefix = %this.clanPrefix;

		%this.loadHasteGame();
		%this.setRank();

		return parent::autoAdminCheck(%this);
	}

	function GameConnection::OnDeath(%client, %killerPlayer, %killer, %damageType, %damageLoc) {
		parent::OnDeath(%client, %killerPlayer, %killer, %damageType, %damageLoc);

		if(!$Haste::Running) {
			return;
		}

		if($Haste::Running) {
			$DefaultMinigame.messageAll('', "\c3" @ %client.name SPC "\c5lasted\c3" SPC getTimeString((getSimTime() - $Haste::StartedAt)/1000) @ "!");
		}
	}

	function MinigameSO::checkLastManStanding(%this) {
		for(%i=0;%i<%this.numMembers;%i++) {
			if(isObject(%this.member[%i].player)) {
				%winner = %this.member[%i];
				%count++;
			}
		}
		if(%count > 1) {
			echo("doing nothing...");
			return parent::checkLastManStanding(%this);
		}
		if(%count == 1) {
			%winner.score++;
			%winner.setRank();
			%this.messageAll('',"\c3" @ %winner.name SPC "\c5won this round! They have won\c3" SPC %winner.score SPC "\c5time(s)!");
			$DefaultMinigame.messageAll('', "\c3" @ %winner.name SPC "\c5lasted\c3" SPC getTimeString((getSimTime() - $Haste::StartedAt)/1000) @ "!");
		}
		if(!$Haste::Running) {
			return;
		}

		setEnvironment("SkyColor", "128 192 255");
		setEnvironment("DirectLightColor", "192 192 192");
		setEnvironment("AmbientLightColor", "128 128 128");
		setEnvironment("ShadowColor", "80 80 80");
		setEnvironment("FogColor", "192 224 255");

		$Haste::Running = 0;

		for(%i=0;%i<%this.numMembers;%i++) {
			cancel(%this.member[%i].bpSched);
			cancel(%this.member[%i].bpSched);
		}
		%this.saveHasteGame();

		schedule(7000, 0, loadMapPhase1);

		return parent::checkLastManStanding(%this);
	}

	function MinigameSO::reset(%this) {
		if(%this.numMembers < 1) {
			return;
		}
		return parent::reset(%this);
	}

	function ServerLoadSaveFile_End() {
		parent::ServerLoadSaveFile_End();

		%mg = $DefaultMinigame;
		%mg.respawnAll();

		$Haste::StartedAt = getSimTime() + 45000+BrickGroup_888888.getCount();

		for(%i=0;%i<%mg.numMembers;%i++) {
			%mg.member[%i].updateBottomPrint();
		}

		updateHasteAudio(0);

		setEnvironment("WaterHeight", 1);
		setEnvironment("WaterIdx", 2);
		setEnvironment("WaterColor", "0 0 0");

		%delay = 45000+BrickGroup_888888.getCount();
		%mg.startSched = schedule(%delay, 0, startRemovingBricks);
		for(%i=1;%i<=5;%i++) {
			%mg.schedule(%delay - (%i*1000), playSound, "hasteCountdownSound");
		}

		%mg.respawnTime = 3000;
	}

	function Player::activateStuff(%this) {
		%this.checkClickedObject();

		return parent::activateStuff(%this);
	}

	function onServerDestroyed() {
		$loadOffset = "0 0 0";
		$Haste::Running = 0;
		$Haste::Init = 0;
		
		%mg = $DefaultMinigame;
		cancel(%mg.startSched);
		cancel($Haste::RemoveSched);
		cancel($Haste::DigSched);
		
		if(isObject(MapDB)) {
			for(%i=0;%i<MapDB.getCount();%i++) {
				MapDB.getObject(0).delete();
			}
			MapDB.delete();
		}
		
		return parent::onServerDestroyed();
	}

};
activatePackage(HastePackage);