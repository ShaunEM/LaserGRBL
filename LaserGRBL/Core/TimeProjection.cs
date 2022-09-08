using LaserGRBL.Libraries.GRBLLibrary;
using Sound;
using System;
using System.Collections.Generic;

namespace LaserGRBL.Core
{
	public class TimeProjection
	{
		private TimeSpan mETarget;
		private TimeSpan mEProgress;


		private long mStart;        //Start Time
		private long mEnd;          //End Time
		private long mGlobalStart;  //Global Start (multiple pass)
		private long mGlobalEnd;    //Global End (multiple pass)
		private long mPauseBegin;   //Pause begin Time
		private long mCumulatedPause;

		private bool mInPause;
		private bool mCompleted;
		private bool mStarted;

		private int mTargetCount;
		private int mExecutedCount;
		private int mSentCount;
		private int mErrorCount;
		private int mContinueCorrection;

		GrblCore.DetectedIssue mLastIssue;
		private GPoint mLastKnownWCO;

		public GPoint LastKnownWCO
		{
			get { return mLastKnownWCO; }
			set { if (InProgram) mLastKnownWCO = value; }
		}

		public TimeProjection()
		{ 
			Reset(true); 
		}

		public void Reset(bool global)
		{
			mETarget = TimeSpan.Zero;
			mEProgress = TimeSpan.Zero;
			mStart = mEnd = 0;
			if (global)
			{
				mGlobalStart = mGlobalEnd = 0;
			}
			mPauseBegin = 0;
			mCumulatedPause = 0;
			mInPause = false;
			mCompleted = false;
			mStarted = false;
			mExecutedCount = 0;
			mSentCount = 0;
			mErrorCount = 0;
			mTargetCount = 0;
			mContinueCorrection = 0;
			mLastIssue = GrblCore.DetectedIssue.Unknown;
			mLastKnownWCO = GPoint.Zero;
		}

		public TimeSpan EstimatedTarget
		{ get { return mETarget; } }

		public bool InProgram
		{ get { return mStarted && !mCompleted; } }

		public int Target
		{ get { return mTargetCount; } }

		public int Sent
		{ get { return mSentCount - mContinueCorrection; } }

		public int Executed
		{ get { return mExecutedCount - mContinueCorrection; } }

		public TimeSpan ProjectedTarget
		{
			get
			{
				if (mStarted)
				{
					double real = TrueJobTime.TotalSeconds; //job time spent in execution
					double target = mETarget.TotalSeconds;  //total estimated
					double done = mEProgress.TotalSeconds;  //done of estimated

					if (done != 0)
						return TimeSpan.FromSeconds(real * target / done) + TotalJobPauses;
					else
						return EstimatedTarget;
				}
				else
					return TimeSpan.Zero;
			}
		}

		private TimeSpan TrueJobTime
		{ get { return TotalJobTime - TotalJobPauses; } }

		public TimeSpan TotalJobTime
		{
			get
			{
				if (mCompleted)
					return TimeSpan.FromMilliseconds(mEnd - mStart);
				else if (mStarted)
					return TimeSpan.FromMilliseconds(now - mStart);
				else
					return TimeSpan.Zero;
			}
		}

		public TimeSpan TotalGlobalJobTime
		{
			get
			{
				if (mCompleted)
					return TimeSpan.FromMilliseconds(mGlobalEnd - mGlobalStart);
				else if (mStarted)
					return TimeSpan.FromMilliseconds(now - mGlobalStart);
				else
					return TimeSpan.Zero;
			}
		}

		private TimeSpan TotalJobPauses
		{
			get
			{
				if (mInPause)
					return TimeSpan.FromMilliseconds(mCumulatedPause + (now - mPauseBegin));
				else
					return TimeSpan.FromMilliseconds(mCumulatedPause);
			}
		}

		public void JobStart(TimeSpan totalEstimatedTime, Queue<GrblCommand> mQueuePtr, bool global)
		{
			if (!mStarted)
			{
				mETarget = totalEstimatedTime;
				mTargetCount = mQueuePtr.Count;
				mEProgress = TimeSpan.Zero;
				mStart = Tools.HiResTimer.TotalMilliseconds;
				if (global)
				{
					mGlobalStart = mStart;
				}
				mPauseBegin = 0;
				mInPause = false;
				mCompleted = false;
				mStarted = true;
				mExecutedCount = 0;
				mSentCount = 0;
				mErrorCount = 0;
				mContinueCorrection = 0;
				mLastIssue = GrblCore.DetectedIssue.Unknown;
				mLastKnownWCO = GPoint.Zero;
			}
		}

		//public void JobContinue(GrblFile file, int position, int added)
		//{
		//	if (!mStarted)
		//	{
		//		if (mETarget == TimeSpan.Zero)
		//		{
		//			mETarget = file.EstimatedTime;
		//		}
		//		if (mTargetCount == 0)
		//		{
		//			mTargetCount = file.GCodeLineCount;
		//		}
		//		//mEProgress = TimeSpan.Zero;
		//		if (mStart == 0)
		//		{
		//			mGlobalStart = mStart = Tools.HiResTimer.TotalMilliseconds;
		//		}

		//		mPauseBegin = 0;
		//		mInPause = false;
		//		mCompleted = false;
		//		mStarted = true;
		//		mExecutedCount = position;
		//		mSentCount = position;
		//		mLastIssue = GrblCore.DetectedIssue.Unknown;
		//		//	mErrorCount = 0;
		//		mContinueCorrection = added;
		//	}
		//}

		public void JobSent()
		{
			if (mStarted && !mCompleted)
				mSentCount++;
		}

		public void JobError()
		{
			if (mStarted && !mCompleted)
			{
				SoundEvent.PlaySound(SoundEvent.EventId.Warning);
				mErrorCount++;
			}
		}

		public void JobExecuted(TimeSpan EstimatedProgress)
		{
			if (mStarted && !mCompleted)
			{
				mExecutedCount++;
				mEProgress = EstimatedProgress;
			}
		}

		public void JobPause()
		{
			if (mStarted && !mCompleted && !mInPause)
			{
				mInPause = true;
				mPauseBegin = now;
			}
		}

		public void JobResume()
		{
			if (mStarted && !mCompleted && mInPause)
			{
				mCumulatedPause += Tools.HiResTimer.TotalMilliseconds - mPauseBegin;
				mInPause = false;
			}
		}

		public bool JobEnd(bool global)
		{
			if (mStarted && !mCompleted)
			{
				JobResume(); //nel caso l'ultimo comando fosse una pausa, la chiudo e la cumulo
				mEnd = Tools.HiResTimer.TotalMilliseconds;
				if (global)
				{
					mGlobalEnd = mEnd;
				}
				mCompleted = true;
				mStarted = false;
				return true;
			}

			return false;
		}

		public void JobIssue(GrblCore.DetectedIssue issue)
		{ mLastIssue = issue; }

		private long now
		{ get { return Tools.HiResTimer.TotalMilliseconds; } }

		public int ErrorCount
		{ get { return mErrorCount; } }

		public GrblCore.DetectedIssue LastIssue
		{ get { return mLastIssue; } }


	}

}
