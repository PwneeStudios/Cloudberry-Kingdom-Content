/*---------------------------------------------------------------------------*

  Copyright 2010-2011 Nintendo.  All rights reserved.

  These coded instructions, statements, and computer programs contain
  proprietary information of Nintendo of America Inc. and/or Nintendo
  Company Ltd., and are protected by Federal copyright law.  They may
  not be disclosed to third parties or copied or duplicated in any form,
  in whole or in part, without the prior written consent of Nintendo.

 *---------------------------------------------------------------------------*/
#extension GL_EXT_Cafe : enable

layout(location=0) in vec4 a_position;
layout(location=1) in vec2 a_texcoord;

varying vec2 v_texcoord;

void main()
{
    gl_Position = a_position;
    v_texcoord = a_texcoord;
}

