$Haste::SaveDir = "config/server/instability/save/";

function MinigameSO::saveHasteGame(%this) {
	%file = new FileObject();

	for(%i=0;%i<%this.numMembers;%i++) {
		%client = %this.member[%i];

		%file.openForWrite($Haste::SaveDir @ %client.bl_id);
		%file.writeLine(%client.score);

		%file.close();
	}

	%file.delete();
}

function GameConnection::loadHasteGame(%this) {
	if(!isFile($Haste::SaveDir @ %this.bl_id)) {
		return -1;
	}

	%file = new FileObject();

	%file.openForRead($Haste::SaveDir @ %this.bl_id);
	%this.score = %file.readLine();

	%file.close();
	%file.delete();
}