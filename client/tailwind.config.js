module.exports = {
    theme: {
        extend: {
            colors: {
                gray: {
                    '100': '#EFEEEA',
                    '200': '#CFCFCE',
                    '300': '#AFB0B1',
                    '400': '#9FA0A3',
                    '500': '#8F9195',
                    '600': '#4F525C',
                    '700': '#2F343F',
                    '800': '#23272F',
                    '900': '#1B1E25'
                },
                red: {
                    '100': '#FFD1D5',
                    '200': '#f7b8bd',
                    '300': '#f09ea5',
                    '400': '#e8858d',
                    '500': '#E06C75',
                    '600': '#D53845',
                    '700': '#AE2530',
                    '800': '#811B24',
                    '900': '#541217'
                },
                green: {
                    '100': '#DDFFC4',
                    '200': '#C5EAA9',
                    '300': '#ACD58F',
                    '400': '#94C074',
                    '500': '#7BAB59',
                    '600': '#679546',
                    '700': '#537F33',
                    '800': '#3E6820',
                    '900': '#2A520D'
                },
                blue: {
                    '100': '#E8F3FD',
                    '200': '#C8E3FA',
                    '300': '#A8D3F6',
                    '400': '#86C2F3',
                    '500': '#61AFEF',
                    '600': '#1789E6',
                    '700': '#126CB5',
                    '800': '#0D4F84',
                    '900': '#083254'
                }
            },
            width: {
                '800px': '800px'
            },
            boxShadow: {
                default: '0 4px 4px rgba(0, 0, 0, 0.25)',
                inner: 'inset 0px 4px 4px rgba(0, 0, 0, 0.25);'
            },
            aspectRatio: {
                square: [1, 1]
            }
        }
    },
    variants: {},
    plugins: [
        function ({theme, variants, e, addUtilities}) {
            const defaultAspectRatioTheme = {};
            const defaultAspectRatioVariants = ['responsive'];

            const aspectRatioTheme = theme('aspectRatio', defaultAspectRatioTheme);
            const aspectRatioVariants = variants('aspectRatio', defaultAspectRatioVariants);

            let aspectRatioUtilities = {};
            for (let key in aspectRatioTheme) {
                if (!aspectRatioTheme.hasOwnProperty(key))
                    continue;

                const value = aspectRatioTheme[key];
                const aspectRatio = Array.isArray(value) ? value[0] / value[1] : value;

                let utilityName = "." + e("aspect-ratio-" + key);
                let percentage = (1 / aspectRatio * 100);

                aspectRatioUtilities[utilityName] = { paddingBottom: percentage + "%" };
            }

            addUtilities(aspectRatioUtilities, aspectRatioVariants);
        }
    ]
};
