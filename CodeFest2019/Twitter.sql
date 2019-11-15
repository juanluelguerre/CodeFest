/****** Object:  Table [dbo].[Twitter]    Script Date: 13/11/2019 21:31:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Twitter](
	[Id] [varchar](100) NOT NULL,
	[Text] [varchar](500) NULL,
	[UserName] [varchar](100) NULL,
	[Score] [float] NULL,
 CONSTRAINT [PK_Twitters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


