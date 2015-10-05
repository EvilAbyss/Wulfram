using System.Threading;
using UnityEngine;
using System;
using System.IO;
using Launchie;

public class LaunchieLogic2 : LaunchieLogic
{
	override protected void Asyncwork()
	{
		_l = new Launchie.Launchie( url, version, debug, forceAdmin );
		_l.setOnError( OnError );

		int check_state = _l.Check();
		
		if( check_state == 1 )
		{
			lgui.setState( 1 );
			DownloadPatch();
		}
		else if( check_state == 0 )
		{
			lgui.setState( 2 );
			lgui.setText( "Game is up to date :)" );
			// there are no updates and you can load your game levels
			// here you can add something like this
			// Application.Loadlevel(1);
		}
	}

	void DownloadReleaseNotesDone()
	{
		string availableVersion = _l.getAvailableVersion();
		string releaseNotes = _l.getReleaseNotes();
		double patchSize = _l.getSize();
		lgui.setText( "There is update " + availableVersion + "; "+ FormatSize( patchSize ) + "\n" + releaseNotes );
	}

	override public void DownloadPatch()
	{
		if( lgui.getState() == 1 )
		{
			_l.setOnProgress( DownloadPatchProgress );
			_l.setOnDone( DownloadPatchDone );

			lgui.setState( 3 );
			string availableVersion = _l.getAvailableVersion();
			string releaseNotes = _l.getReleaseNotes();
			double patchSize = _l.getSize();
			lgui.setText( "Downloading " + availableVersion + "; "+ FormatSize( patchSize ) + "\n" + releaseNotes );
			_l.Download();
		}
	}

	void DownloadPatchDone()
	{
		_progress = 0;
		lgui.setProgress( _progress );
		_l.setOnProgress( ExtractProgress );
		_l.setOnDone( ExtractDone );


		lgui.setState( 4 );
		string availableVersion = _l.getAvailableVersion();
		string releaseNotes = _l.getReleaseNotes();
		double patchSize = _l.getSize();
		lgui.setText( "Unpacking " + availableVersion + "; "+ FormatSize( patchSize ) + "\n" + releaseNotes );
		_l.Extract();
	}


	DateTime last_time_speed_check = DateTime.Now;
	double last_time_progress ;
	string speed;
	void DownloadPatchProgress( double progress )
	{
		lgui.setProgress( progress );

		double diff = ( DateTime.Now - last_time_speed_check ).TotalMilliseconds;

		if( diff > 100 )
		{
			last_time_speed_check = DateTime.Now;
			speed = FormatSize( ( progress - last_time_progress ) / 100 * _l.getSize() * 1000 / diff ) + "/s";
			last_time_progress = progress;
		}

		string availableVersion = _l.getAvailableVersion();
		string releaseNotes = _l.getReleaseNotes();
		double patchSize = _l.getSize();

		// if you set this to false it will count from 0 to size of patch
		// if true it will show remaining bytes
		bool remaining_size = true;
		if( remaining_size )
		{
			lgui.setText( "Downloading " + availableVersion + "; "+speed+"; "+ FormatSize( patchSize * ( 100 - progress ) / 100 ) + "\n" + releaseNotes );
		}
		else
		{
			lgui.setText( "Downloading " + availableVersion + "; "+speed+"; "+ FormatSize( patchSize * ( progress / 100 ) ) + "\n" + releaseNotes );
		}
	}

	void ExtractProgress( double progress )
	{
		lgui.setProgress( progress );
	}

	void ExtractDone()
	{
		_l.setCurrentVersion( _l.getAvailableVersion() );
		if( _l.ForceCheck() == 1 )
		{
			lgui.setState( 1 );
			DownloadPatch();
		}
		else
		{
			_l.Finish();
			lgui.setState( 5 );
		}
	}
}