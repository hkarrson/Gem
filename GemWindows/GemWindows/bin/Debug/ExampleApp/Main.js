this.App = function()
{
	this.Test = function()
	{
		pause();
		reload();
	}
}
this.__App__ = new this.App(); // App is a class; it contains methods

log(Reloaded);
log(blah);
let blah = "blablablah!";
__App__.Test();