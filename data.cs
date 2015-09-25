datablock AudioProfile(musicData_GameAmbiance) {
	fileName = "./sounds/game_amb.ogg";
	description = AudioMusicLooping3d;
	preload = 1;
	uiName = "Haste Game Ambiance";
};

datablock AudioProfile(musicData_PreGameAmbiance) {
	fileName = "./sounds/pre_amb.ogg";
	description = AudioMusicLooping3d;
	preload = 1;
	uiName = "Haste Pre-game Ambiance";
};

if(!isObject(HasteAudioEmitter)) {
	new AudioEmitter(HasteAudioEmitter) {
		is3D = 0;
		isLooping = 1;
		loopCount = -1;
		maxDistance = 999999;
		position = "0 0 0";
		profile = "musicData_PreGameAmbiance";
		volume = 1;
	};
}

datablock AudioProfile(hasteCountdownSound) {
	filename = "./sounds/countdown.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(hasteStartSound) {
	filename = "./sounds/start.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(hasteClickSound) {
	filename = "./sounds/click.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(hastePlantSound1) {
	filename = "./sounds/plant1.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(hastePlantSound2 : hastePlantSound1) { filename = "./sounds/plant2.wav"; };
datablock AudioProfile(hastePlantSound3 : hastePlantSound1) { filename = "./sounds/plant3.wav"; };
datablock AudioProfile(hastePlantSound4 : hastePlantSound1) { filename = "./sounds/plant4.wav"; };
datablock AudioProfile(hastePlantSound5 : hastePlantSound1) { filename = "./sounds/plant5.wav"; };
datablock AudioProfile(hastePlantSound6 : hastePlantSound1) { filename = "./sounds/plant6.wav"; };
datablock AudioProfile(hastePlantSound7 : hastePlantSound1) { filename = "./sounds/plant7.wav"; };
datablock AudioProfile(hastePlantSound8 : hastePlantSound1) { filename = "./sounds/plant8.wav"; };

function updateHasteAudio(%which) {
	if(isObject(HasteAudioEmitter)) {
		HasteAudioEmitter.delete();
	}

	%db = "musicData_PreGameAmbiance";
	switch(%which) {
		case 0:
			%db = "musicData_PreGameAmbiance";
		case 1:
			%db = "musicData_GameAmbiance";
	}

	new AudioEmitter(HasteAudioEmitter) {
		is3D = 0;
		isLooping = 1;
		loopCount = -1;
		maxDistance = 999999;
		position = BrickGroup_888888.getObject(getRandom(0, BrickGroup_888888.getCount()-1)).getPosition();
		profile = %db;
		volume = 1;
	};
}

datablock ProjectileData(HasteAsteroidProjectile : RocketLauncherProjectile) {
	className = "HasteAsteroid";
};