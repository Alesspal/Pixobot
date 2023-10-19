﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DobotClientDemo.CPlusDll
{

    /*
     * 末端选择
     * list of end effector type
     */
    public enum EndType
    {
        EndTypeCustom,
        EndTypeSuctionCap,
        EndTypeGripper,
        EndTypeLaser,
        EndTypePen,
        EndTypeMax,
        EndTypeNum = EndTypeMax
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct EndTypeParams
    {
        public float xBias;
        public float yBias;
        public float zBias;
    };

    /*
     * 位姿
     * pose struct
     */
    public struct Pose
    {
        public float x;
        public float y;
        public float z;
        public float rHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] jointAngle;
    };

    /*
     * 运动学参数
     * motion parameters
     */
    public struct Kinematics
    {
        public float velocity;
        public float acceleration;
    };

    /*
     * 报警状态：暂时支持32种报警状态
     * AlarmStatus: currently support 32 alarm states
     */
    public struct AlarmsState
    {
        public int alarmsState;
    };

    /*********************************************************************************************************
    ** HOME参数 Home Parameters
    *********************************************************************************************************/
    public struct HOMEParams
    {
        public float x;
        public float y;
        public float z;
        public float r;
    };

    public struct HOMECmd
    {
        public int temp;
    };

    /*********************************************************************************************************
    ** 点动示教部分 Joint Control Section
    *********************************************************************************************************/
    /*
     * 单关节点动示教参数
     * JOG Joint Motion Parameters
     */
    public struct JOGJointParams
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] acceleration;
    };

    /*
     * 单坐标轴点动示教参数
     * JOG Coordinate Motion Parameters
     */
    public struct JOGCoordinateParams
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] acceleration;
    };

    /*
     * 点动示教公共参数
     * JOG Common Parameters
     */
    public struct JOGCommonParams
    {
        public float velocityRatio;
        public float accelerationRatio;
    };

    /*
     * Jog 指令
     * Jog Cmd
     */
    public enum JogCmdType
    {
        Idle,
        X_Plus,
        X_Minus,
        Y_Plus,
        Y_Minus,
        Z_Plus,
        Z_Minus,
        R_Plus,
        R_Minus,
        L_Plus,
        L_Minus
    };

    /*
     * Jog 即使运动指令
     * Jog instant cmd
     */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct JogCmd
    {
        public byte isJoint;
        public byte cmd;
    };

    /*********************************************************************************************************
    ** 再现运动部分 Motion PlayBack Section
    *********************************************************************************************************/
    /*
     * 再现运动参数 Motion PlayBack Parameters
     */
    public struct PTPJointParams
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] acceleration;
    };
    public struct PTPCoordinateParams
    {
        public float xyzVelocity;
        public float rVelocity;
        public float xyzAcceleration;
        public float rAcceleration;
    };

    public struct PTPJumpParams
    {
        public float jumpHeight;
        public float zLimit;
    };

    public struct PTPCommonParams
    {
        public float velocityRatio;
        public float accelerationRatio;
    };

    // For play back
    public enum PTPMode
    {
        PTPJUMPXYZMode,
        PTPMOVJXYZMode,
        PTPMOVLXYZMode,

        PTPJUMPANGLEMode,
        PTPMOVJANGLEMode,
        PTPMOVLANGLEMode,

        PTPMOVJXYZINCMode,
        PTPMOVLXYZINCMode,
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PTPCmd
    {
        public byte ptpMode;
        public float x;
        public float y;
        public float z;
        public float rHead;
    };

    /*********************************************************************************************************
    ** 连续轨迹：Continuous path
    *********************************************************************************************************/
    /*
     * Continuous Path Parameters
     * CP参数
     */
    public struct CPParams
    {
        public float planAcc;
        public float juncitionVel;
        public float acc;
        public byte realTimeTrack;
    };

    public enum ContinuousPathMode
    {
        CPRelativeMode,
        CPAbsoluteMode
    };
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct CPCmd
    {
        public byte cpMode;
        public float x;
        public float y;
        public float z;
        public float velocity;
    };


    /*********************************************************************************************************
    ** 用户参数 User parameters
    *********************************************************************************************************/

    public struct WAITCmd
    {
        public UInt32 timeout;
    };

    public enum IOFunction
    {
        IOFunctionDummy,
        IOFunctionDO,
        IOFunctionPWM,
        IOFunctionDI,
        IOFunctionADC
    };

    public struct IOMultiplexing
    {
        public byte address;
        public byte multiplex;
    };

    public struct IODO
    {
        public byte address;
        public byte level;
    };

    public struct IOPWM
    {
        public byte address;
        public float frequency;
        public float dutyCycle;
    };

    public struct IODI
    {
        public byte address;
        public byte level;
    };

    public struct IOADC
    {
        public byte address;
        public UInt16 value;
    };

    /*
     * ARC 参数
     * ARC related
     */
    public struct ARCParams
    {
        public float xyzVelocity;
        public float rVelocity;
        public float xyzAcceleration;
        public float rAcceleration;
    };

    public struct ARCCmd
    {
        public float cirPoint_x;
        public float cirPoint_y;
        public float cirPoint_z;
        public float cirPoint_r;

        public float toPoint_x;
        public float toPoint_y;
        public float toPoint_z;
        public float toPoint_r;
    };

    /*********************************************************************************************************
    **用户参数 
    **User parameters
    *********************************************************************************************************/
    public struct UserParams
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] param;
    };

    /*********************************************************************************************************
    ** API 返回值定义
    ** API result
    *********************************************************************************************************/
    public enum DobotConnect
    {
        DobotConnect_NoError,
        DobotConnect_NotFound,
        DobotConnect_Occupied
    };

    public enum DobotCommunicate
    {
        DobotCommunicate_NoError,
        DobotCommunicate_BufferFull,
        DobotCommunicate_Timeout,
        DobotCommunicate_InvalidParams
    };

    //-------------------------------------------------------------- AJOUT

    public enum InfraredPort
    {
        IF_PORT_GP1,
        IF_PORT_GP2,
        IF_PORT_GP4,
        IF_PORT_GP5,
    };
    public enum ColorPort
    {
        IF_PORT_GP1,
        IF_PORT_GP2,
        IF_PORT_GP4,
        IF_PORT_GP5,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct tagEMotor
    {
        public byte index;
        public byte isEnable;
        public int speed;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct tagEMotorS
    {
        public byte index;
        public byte isEnable;
        public int speed;
        public int distance;
    }

    public struct JOGLParams
    {
        public float velocity;
        public float acceleration;
    };

    public struct tagAutoLevelingCmd
    {
        public byte controlFlag; //Enabe Flag
        public float precision; //Leveling precision, the minimum is 0.02
    }

}

