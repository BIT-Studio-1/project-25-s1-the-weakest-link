{
    "startRoom": {
        "connections": {
            "sideRoom": {
                "requirements": [],
                "command": "enter side room"
            },
            "hallway1": {
                "requirements": [],
                "command": "exit main entrance"
            },
            "hallway4": {
                "requirements": ["tablet"],
                "command": "exit second entrance"
            }
        },
        "items": ["book"]
    },
    "sideRoom": {},
    "hallway1": {
        "connections": {
            "startRoom": {
                "requirements": [],
                "command": "enter starting room"
            },
            "vinesRoom": {
                "requirements": [],
                "command": "enter small room"
            },
            "knifeRoom": {
                "requirements": [],
                "command": "enter open room"
            },
            "tabletRoom": {
                "requirements": ["tablet"],
                "command": "enter locked door"
            },
            "hallway2": {
                "requirements": ["tablet"],
                "command": "go south"
            }
        }
    },
    "knifeRoom": {
        "connections": {
            "hallway1": {
                "requirements": [],
                "command": "exit room"
            }
        },
        "items": ["book"]
    },
    "vinesRoom": {
        "connections": {
            "hallway1": {
                "requirements": [],
                "command": "exit main entrance"
            },
            "tabletRoom": {
                "requirements": ["cut vines"],
                "command": "enter main room"
            }
        }
    },
    "tabletRoom": {
        "connections": {
            "vinesRoom": {
                "requirements": [],
                "command": "enter side room"
            },
            "hallway1": {
                "requirements": [],
                "command": "exit main entrance"
            }
        },
        "items": ["tablet"]
    },
    "hallway2": {
        "connections": {
            "stairs": {
                "requirements": ["key"],
                "command": "enter glass door"
            },
            "hallway1": {
                "requirements": [],
                "command": "go north"
            },
            "hallway3": {
                "requirements": [],
                "command": "go west"
            }
        }
    },
    "hallway3": {
        "connections": {
            "hallway2": {
                "requirements": [],
                "command": "go east"
            },
            "hallway4": {
                "requirements": [],
                "command": "go north"
            }
        }
    },
    "hallway4": {
        "connections": {
            "startRoom": {
                "requirements": ["tablet"],
                "command": "enter starting room"
            },
            "keyRoom": {
                "requirements": ["tablet"],
                "connections": "enter locked door"
            },
            "hallway3": {
                "requirements": [],
                "command": "go south"
            }
        }
    },
    "keyRoom": {
        "connections": {
            "hallway4": {
                "requirements": [],
                "command": "exit room"
            }
        },
        "items": ["key"]
    }
}